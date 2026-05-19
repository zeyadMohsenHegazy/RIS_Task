namespace POS.Application.DTOs;

public sealed class InvoiceSummaryDto
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CashierName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public decimal FinalAmount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public int ItemCount { get; init; }
    public string SyncStatus { get; init; } = string.Empty;
}
