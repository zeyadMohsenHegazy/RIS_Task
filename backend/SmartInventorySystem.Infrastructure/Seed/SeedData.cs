using SmartInventorySystem.Domain.Constants;

namespace SmartInventorySystem.Infrastructure.Seed;

internal static class SeedData
{
    public const string AdminUsername = "admin";
    public const string AdminPassword = "Admin@123";
    public const string EmployeeUsername = "employee";
    public const string EmployeePassword = "Employee@123";

    public static IReadOnlyList<WarehouseSeed> Warehouses { get; } =
    [
        new("Cairo Warehouse", "October City, Giza, Egypt"),
        new("Alex Warehouse", "Amreya, Alexandria, Egypt")
    ];

    public static IReadOnlyList<ProductSeed> Products { get; } =
    [
        new("Dell Laptop", "DELL-LAT-5540", 1299.99m, 25, 0),
        new("Iphone 14 Pro Max", "IPH-14-PRO-MAX", 349.99m, 40, 0),
        new("Samsung Monitor", "SAM-MON-27", 279.99m, 30, 0),
        new("Office Chair", "FUR-CHAIR-ERG", 449.99m, 20, 1),
        new("Standing Desk", "FUR-DESK-160", 799.99m, 12, 1)
    ];

    public static IReadOnlyList<TransactionSeed> InventoryTransactions { get; } =
    [
        new(0, 30, true, Roles.Admin),
        new(0, 5, false, Roles.Employee),
        new(1, 50, true, Roles.Admin),
        new(1, 10, false, Roles.Employee),
        new(2, 80, true, Roles.Admin),
        new(2, 20, false, Roles.Employee)
    ];

    internal static void EnsureValid()
    {
        if (Warehouses.Count == 0)
        {
            throw new InvalidOperationException("SeedData must define at least one warehouse.");
        }

        var skus = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < Products.Count; i++)
        {
            var product = Products[i];
            if (product.WarehouseIndex < 0 || product.WarehouseIndex >= Warehouses.Count)
            {
                throw new InvalidOperationException(
                    $"Product '{product.Name}' has invalid WarehouseIndex {product.WarehouseIndex}.");
            }

            if (!skus.Add(product.Sku))
            {
                throw new InvalidOperationException($"Duplicate SKU '{product.Sku}' in seed products.");
            }

            if (product.Price <= 0)
            {
                throw new InvalidOperationException($"Product '{product.Name}' must have Price > 0.");
            }

            if (product.Quantity < 0)
            {
                throw new InvalidOperationException($"Product '{product.Name}' must have Quantity >= 0.");
            }
        }

        for (var i = 0; i < InventoryTransactions.Count; i++)
        {
            var transaction = InventoryTransactions[i];
            if (transaction.ProductIndex < 0 || transaction.ProductIndex >= Products.Count)
            {
                throw new InvalidOperationException(
                    $"Transaction #{i} has invalid ProductIndex {transaction.ProductIndex}.");
            }

            if (transaction.Quantity <= 0)
            {
                throw new InvalidOperationException($"Transaction #{i} must have Quantity > 0.");
            }

            if (!IsSupportedUserKey(transaction.UserKey))
            {
                throw new InvalidOperationException(
                    $"Transaction #{i} has invalid UserKey '{transaction.UserKey}'. Use '{Roles.Admin}' or '{Roles.Employee}'.");
            }
        }

        if (AdminPassword.Length < 6 || EmployeePassword.Length < 6)
        {
            throw new InvalidOperationException("Seed passwords must be at least 6 characters.");
        }
    }

    internal static bool IsSupportedUserKey(string userKey) =>
        userKey.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase)
        || userKey.Equals(Roles.Employee, StringComparison.OrdinalIgnoreCase);

    internal static string ResolveUsername(string userKey) =>
        userKey.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase)
            ? AdminUsername
            : userKey.Equals(Roles.Employee, StringComparison.OrdinalIgnoreCase)
                ? EmployeeUsername
                : throw new InvalidOperationException($"Unknown seed user key: {userKey}");

    internal sealed record WarehouseSeed(string Name, string Location);

    internal sealed record ProductSeed(
        string Name,
        string Sku,
        decimal Price,
        int Quantity,
        int WarehouseIndex);

    internal sealed record TransactionSeed(
        int ProductIndex,
        int Quantity,
        bool IsStockIn,
        string UserKey);
}