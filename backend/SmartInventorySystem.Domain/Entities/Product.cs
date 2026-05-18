namespace SmartInventorySystem.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int WarehouseId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Warehouse? Warehouse { get; set; }
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } =
        new List<InventoryTransaction>();

    protected Product() { }

    public Product(
        string name,
        string sku,
        decimal price,
        int quantity,
        int warehouseId,
        DateTime? createdAt = null)
    {
        Name = name;
        SKU = sku;
        Price = price;
        Quantity = quantity;
        WarehouseId = warehouseId;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }
}
