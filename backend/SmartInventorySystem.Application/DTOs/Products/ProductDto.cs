namespace SmartInventorySystem.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public DateTime CreatedAt { get; set; }
}
