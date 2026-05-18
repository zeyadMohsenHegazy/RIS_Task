using POS.Domain.Enums;

namespace POS.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal FinalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public SyncStatus SyncStatus { get; set; } = SyncStatus.NotRequired;
    public DateTime? SyncedAt { get; set; }
    public string? SyncError { get; set; }

    public User User { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
