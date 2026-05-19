namespace POS.Application.DTOs;

public sealed class InvoiceDetailDto
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
    public string SyncStatus { get; init; } = string.Empty;
    public DateTime? SyncedAt { get; init; }
    public string? SyncError { get; init; }
    public IReadOnlyList<InvoiceItemDetailDto> Items { get; init; } = Array.Empty<InvoiceItemDetailDto>();
}
