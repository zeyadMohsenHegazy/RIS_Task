using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Enums;

namespace POS.Infrastructure.Data;

public sealed class DatabaseInitializer
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly DatabaseOptions _options;

    public DatabaseInitializer(
        IDatabaseConnectionFactory connectionFactory,
        DatabaseOptions options)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = _connectionFactory.CreateDbContext();

        if (_options.AutoMigrate)
        {
            await context.Database.MigrateAsync(cancellationToken);
        }

        if (_options.SeedDefaultUsers && !await context.Users.AnyAsync(cancellationToken))
        {
            context.Users.AddRange(
                new User
                {
                    Username = "cashier",
                    PasswordHash = "b4c94003c562bb0d89535eca77f07284fe560fd48a7cc1ed99f0a56263d616ba",
                    Role = UserRole.Cashier
                },
                new User
                {
                    Username = "manager",
                    PasswordHash = "866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5",
                    Role = UserRole.Manager
                });

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
