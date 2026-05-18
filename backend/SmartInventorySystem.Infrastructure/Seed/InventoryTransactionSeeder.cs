using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Domain.Enums;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Seed;

internal static class InventoryTransactionSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IReadOnlyList<Product> products,
        IReadOnlyDictionary<string, ApplicationUser> users,
        CancellationToken cancellationToken)
    {
        if (await context.InventoryTransactions.AnyAsync(cancellationToken))
        {
            return;
        }

        var baseDate = DateTime.UtcNow.AddDays(-14);
        var transactions = new List<InventoryTransaction>();

        for (var i = 0; i < SeedData.InventoryTransactions.Count; i++)
        {
            var seed = SeedData.InventoryTransactions[i];
            var userKey = seed.UserKey.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                ? SeedData.AdminUsername
                : SeedData.EmployeeUsername;
            var user = users[userKey];
            var type = seed.IsStockIn ? TransactionType.In : TransactionType.Out;

            transactions.Add(new InventoryTransaction(
                products[seed.ProductIndex].Id,
                seed.Quantity,
                type,
                user.Id,
                baseDate.AddDays(i / 2).AddHours(i % 8)));
        }

        await context.InventoryTransactions.AddRangeAsync(transactions, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
