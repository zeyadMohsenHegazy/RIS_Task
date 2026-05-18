using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Seed;

internal static class ProductSeeder
{
    public static async Task<IReadOnlyList<Product>> SeedAsync(
        ApplicationDbContext context,
        IReadOnlyList<Warehouse> warehouses,
        CancellationToken cancellationToken)
    {
        if (await context.Products.AnyAsync(cancellationToken))
        {
            return await context.Products
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        var seedTime = DateTime.UtcNow.AddDays(-30);
        var products = SeedData.Products
            .Select((p, index) =>
            {
                var warehouseId = warehouses[p.WarehouseIndex].Id;
                var product = new Product(p.Name, p.Sku, p.Price, p.Quantity, warehouseId, seedTime.AddDays(index));
                return product;
            })
            .ToList();

        await context.Products.AddRangeAsync(products, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return products;
    }
}
