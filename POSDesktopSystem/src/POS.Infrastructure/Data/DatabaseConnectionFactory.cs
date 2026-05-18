using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace POS.Infrastructure.Data;

public interface IDatabaseConnectionFactory
{
    DatabaseProviderType Provider { get; }

    bool RequiresSync { get; }

    string GetConnectionString();

    string? GetRemoteConnectionString();

    void ConfigureDbContext(DbContextOptionsBuilder<POSDbContext> options);

    POSDbContext CreateDbContext();

    POSDbContext CreateRemoteDbContext();

    DbContextOptions<POSDbContext> CreateOptions();
}

public sealed class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly DatabaseProviderType _provider;
    private readonly string _connectionString;
    private readonly string? _remoteConnectionString;

    public DatabaseConnectionFactory(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? new DatabaseOptions();

        _provider = ParseProvider(databaseOptions.Provider);
        _connectionString = ResolveConnectionString(configuration, databaseOptions);
        _remoteConnectionString = configuration.GetConnectionString("RemoteConnection");

        RequiresSync = _provider == DatabaseProviderType.Sqlite
            && !string.IsNullOrWhiteSpace(_remoteConnectionString);
    }

    public DatabaseProviderType Provider => _provider;

    public bool RequiresSync { get; }

    public string GetConnectionString() => _connectionString;

    public string? GetRemoteConnectionString() => _remoteConnectionString;

    public void ConfigureDbContext(DbContextOptionsBuilder<POSDbContext> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        switch (_provider)
        {
            case DatabaseProviderType.Sqlite:
                options.UseSqlite(_connectionString);
                break;
            case DatabaseProviderType.SqlServer:
            default:
                options.UseSqlServer(_connectionString);
                break;
        }
    }

    public DbContextOptions<POSDbContext> CreateOptions()
    {
        var builder = new DbContextOptionsBuilder<POSDbContext>();
        ConfigureDbContext(builder);
        return builder.Options;
    }

    public POSDbContext CreateDbContext() => new(CreateOptions());

    public POSDbContext CreateRemoteDbContext()
    {
        if (string.IsNullOrWhiteSpace(_remoteConnectionString))
        {
            throw new InvalidOperationException(
                "Remote connection string is not configured. Set ConnectionStrings:RemoteConnection in appsettings.json.");
        }

        var options = new DbContextOptionsBuilder<POSDbContext>()
            .UseSqlServer(_remoteConnectionString)
            .Options;

        return new POSDbContext(options);
    }

    private static DatabaseProviderType ParseProvider(string? provider) =>
        provider?.Trim().ToLowerInvariant() switch
        {
            "sqlite" => DatabaseProviderType.Sqlite,
            _ => DatabaseProviderType.SqlServer
        };

    private static string ResolveConnectionString(IConfiguration configuration, DatabaseOptions databaseOptions)
    {
        if (databaseOptions.Provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            var sqlitePath = databaseOptions.SqliteFileName;
            if (!Path.IsPathRooted(sqlitePath))
            {
                sqlitePath = Path.Combine(AppContext.BaseDirectory, "Data", sqlitePath);
            }

            var directory = Path.GetDirectoryName(sqlitePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return $"Data Source={sqlitePath}";
        }

        return configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found in configuration.");
    }
}
