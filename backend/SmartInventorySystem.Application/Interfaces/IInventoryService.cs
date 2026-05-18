using SmartInventorySystem.Application.DTOs.Inventory;

namespace SmartInventorySystem.Application.Interfaces;

public interface IInventoryService
{
    Task<InventoryTransactionDto> StockInAsync(
        InventoryMovementDto dto,
        CancellationToken cancellationToken = default);

    Task<InventoryTransactionDto> StockOutAsync(
        InventoryMovementDto dto,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryTransactionDto>> GetHistoryAsync(
        CancellationToken cancellationToken = default);
}
