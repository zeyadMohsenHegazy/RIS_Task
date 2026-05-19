using POS.Domain.Enums;

namespace POS.Application.Models;

public sealed class InvoiceSearchRow
{
    public int Id { get; init; }
    public DateTime Date { get; init; }
    public DateTime CreatedAt { get; init; }
    public string CashierName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public decimal FinalAmount { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public int ItemCount { get; init; }
    public SyncStatus SyncStatus { get; init; }
}
