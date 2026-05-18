using SmartInventorySystem.Application.DTOs.Inventory;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Mappings;

public static class InventoryMapper
{
    public static InventoryTransactionDto ToDto(InventoryTransaction transaction)
    {
        return new InventoryTransactionDto
        {
            Id = transaction.Id,
            ProductId = transaction.ProductId,
            ProductName = transaction.Product?.Name ?? string.Empty,
            ProductSku = transaction.Product?.SKU ?? string.Empty,
            Quantity = transaction.Quantity,
            TransactionType = transaction.TransactionType,
            TransactionDate = transaction.TransactionDate,
            CreatedByUserId = transaction.CreatedByUserId,
            CreatedByUsername = transaction.CreatedByUser?.Username ?? string.Empty
        };
    }

    public static IReadOnlyList<InventoryTransactionDto> ToDtoList(
        IEnumerable<InventoryTransaction> transactions)
    {
        return transactions.Select(ToDto).ToList();
    }
}
