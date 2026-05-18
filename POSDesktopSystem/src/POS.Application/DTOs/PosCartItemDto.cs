namespace POS.Application.DTOs;

public sealed class PosCartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int AvailableStock { get; set; }
    public decimal SubTotal => Quantity * UnitPrice;
}
