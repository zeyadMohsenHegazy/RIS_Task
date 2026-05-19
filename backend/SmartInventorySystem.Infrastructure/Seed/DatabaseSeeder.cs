using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Seed;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        SeedData.EnsureValid();

        if (await IsSeedCompleteAsync(cancellationToken))
        {
            _logger.LogInformation("Database seed skipped — sample data already exists.");
            return;
        }

        _logger.LogInformation("Starting database seed...");

        var users = await UserSeeder.SeedAsync(_context, _passwordHasher, cancellationToken);
        var warehouses = await WarehouseSeeder.SeedAsync(_context, cancellationToken);
        var products = await ProductSeeder.SeedAsync(_context, warehouses, cancellationToken);
        await InventoryTransactionSeeder.SeedAsync(_context, products, users, cancellationToken);

        _logger.LogInformation(
            "Database seed completed: {WarehouseCount} warehouses, {ProductCount} products, {UserCount} users, {TransactionCount} transactions.",
            warehouses.Count,
            products.Count,
            users.Count,
            SeedData.InventoryTransactions.Count);
    }

    private async Task<bool> IsSeedCompleteAsync(CancellationToken cancellationToken)
    {
        return await _context.Warehouses.AnyAsync(cancellationToken)
            && await _context.Products.CountAsync(cancellationToken) >= SeedData.Products.Count;
    }
}
