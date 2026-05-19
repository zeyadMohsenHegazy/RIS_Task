namespace POS.Application.DTOs;

public sealed class InvoiceCalculationDto
{
    public decimal TotalAmount { get; init; }
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public decimal FinalAmount { get; init; }
}
