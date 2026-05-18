using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Seed;

internal static class WarehouseSeeder
{
    public static async Task<IReadOnlyList<Warehouse>> SeedAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        if (await context.Warehouses.AnyAsync(cancellationToken))
        {
            return await context.Warehouses
                .OrderBy(w => w.Id)
                .ToListAsync(cancellationToken);
        }

        var warehouses = SeedData.Warehouses
            .Select(w => new Warehouse(w.Name, w.Location))
            .ToList();

        await context.Warehouses.AddRangeAsync(warehouses, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return warehouses;
    }
}
