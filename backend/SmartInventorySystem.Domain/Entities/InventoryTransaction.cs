using SmartInventorySystem.Domain.Enums;

namespace SmartInventorySystem.Domain.Entities;

public class InventoryTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public int CreatedByUserId { get; set; }

    public Product? Product { get; set; }
    public ApplicationUser? CreatedByUser { get; set; }

    protected InventoryTransaction() { }

    public InventoryTransaction(
        int productId,
        int quantity,
        TransactionType transactionType,
        int createdByUserId,
        DateTime? transactionDate = null)
    {
        ProductId = productId;
        Quantity = quantity;
        TransactionType = transactionType;
        CreatedByUserId = createdByUserId;
        TransactionDate = transactionDate ?? DateTime.UtcNow;
    }
}
