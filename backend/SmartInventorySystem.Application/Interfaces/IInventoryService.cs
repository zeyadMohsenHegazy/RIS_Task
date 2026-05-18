using SmartInventorySystem.Application.DTOs.Common;
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

    Task<PagedResponse<InventoryTransactionDto>> GetHistoryAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
}
