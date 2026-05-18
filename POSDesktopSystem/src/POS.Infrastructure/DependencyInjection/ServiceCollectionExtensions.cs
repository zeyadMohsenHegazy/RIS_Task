using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Application.Interfaces;
using POS.Infrastructure.Bootstrap;
using POS.Infrastructure.Data;
using POS.Infrastructure.Logging;
using POS.Infrastructure.Repositories;
using POS.Infrastructure.Sync;

namespace POS.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? new DatabaseOptions();
        var syncOptions = configuration.GetSection(SyncOptions.SectionName).Get<SyncOptions>()
            ?? new SyncOptions();

        services.AddSingleton(databaseOptions);
        services.AddSingleton(syncOptions);
        services.AddSingleton<IDatabaseConnectionFactory>(
            _ => new DatabaseConnectionFactory(configuration));
        services.AddSingleton<IDatabaseInfo, DatabaseInfo>();
        services.AddSingleton<IApplicationBootstrap, ApplicationBootstrap>();
        services.AddSingleton<ISystemStatusProvider, SystemStatusProvider>();

        services.AddDbContext<POSDbContext>((serviceProvider, options) =>
        {
            var connectionFactory = serviceProvider.GetRequiredService<IDatabaseConnectionFactory>();
            var connectionString = connectionFactory.GetConnectionString();

            if (connectionFactory.Provider == DatabaseProviderType.Sqlite)
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });

        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<IInvoiceSyncService, EfInvoiceSyncService>();
        services.AddSingleton<BackgroundSyncService>();

        services.AddLogging(configuration);
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection(LoggerSettings.SectionName).Get<LoggerSettings>() ?? new LoggerSettings();
        var logDirectory = Path.IsPathRooted(settings.LogDirectory)
            ? settings.LogDirectory
            : Path.Combine(AppContext.BaseDirectory, settings.LogDirectory);

        services.AddSingleton<IAppLogger>(_ => new FileLogger(logDirectory, settings.FileNamePrefix));
        return services;
    }
}
