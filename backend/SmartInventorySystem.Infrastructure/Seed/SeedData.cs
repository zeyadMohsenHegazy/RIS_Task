namespace SmartInventorySystem.Infrastructure.Seed;

internal static class SeedData
{
    public const string AdminUsername = "admin";
    public const string AdminPassword = "Admin@123";
    public const string EmployeeUsername = "employee";
    public const string EmployeePassword = "Employee@123";

    public static IReadOnlyList<WarehouseSeed> Warehouses { get; } =
    [
        new("Cairo Main Distribution Center", "Industrial Zone, 6th of October City, Giza, Egypt"),
        new("Alexandria Regional Warehouse", "Amreya Industrial Park, Alexandria, Egypt")
    ];

    public static IReadOnlyList<ProductSeed> Products { get; } =
    [
        new("Dell Latitude 5540 Laptop", "DELL-LAT-5540", 1299.99m, 25, 0),
        new("HP LaserJet Pro M404dn", "HP-PRINT-M404", 349.99m, 40, 0),
        new("Logitech MX Master 3S Mouse", "LOG-MX3S", 99.99m, 60, 0),
        new("Samsung 27\" Business Monitor", "SAM-MON-27", 279.99m, 30, 0),
        new("Cisco 24-Port Gigabit Switch", "CIS-SW-24", 599.99m, 15, 0),
        new("Ergonomic Office Chair Pro", "FUR-CHAIR-ERG", 449.99m, 20, 1),
        new("Electric Standing Desk 160cm", "FUR-DESK-160", 799.99m, 12, 1),
        new("Zebra Warehouse Barcode Scanner", "WH-SCAN-ZB", 189.99m, 35, 1),
        new("A4 Copy Paper Box (5 Reams)", "SUP-PAPER-A4", 24.99m, 200, 1),
        new("High-Visibility Safety Vest (10-Pack)", "SAF-VEST-10", 89.99m, 50, 1)
    ];

    /// <summary>
    /// Product index, quantity, transaction type (In/Out), user key (Admin/Employee).
    /// </summary>
    public static IReadOnlyList<TransactionSeed> InventoryTransactions { get; } =
    [
        new(0, 30, true, "Admin"),
        new(0, 5, false, "Employee"),
        new(1, 50, true, "Admin"),
        new(1, 10, false, "Employee"),
        new(2, 80, true, "Admin"),
        new(2, 20, false, "Employee"),
        new(3, 40, true, "Admin"),
        new(4, 20, true, "Admin"),
        new(4, 5, false, "Employee"),
        new(5, 25, true, "Admin"),
        new(6, 15, true, "Admin"),
        new(6, 3, false, "Employee"),
        new(7, 45, true, "Admin"),
        new(8, 250, true, "Admin"),
        new(8, 50, false, "Employee"),
        new(9, 60, true, "Admin"),
        new(9, 10, false, "Employee")
    ];

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
