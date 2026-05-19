namespace POS.Application.DTOs;

public sealed class ProductUpsertDto
{
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
}
