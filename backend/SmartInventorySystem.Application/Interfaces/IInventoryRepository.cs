using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Domain.Enums;

namespace SmartInventorySystem.Application.Interfaces;

public interface IInventoryRepository : IGenericRepository<InventoryTransaction>
{
    Task<PagedResult<InventoryTransaction>> GetHistoryPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        TransactionType? transactionType = null,
        CancellationToken cancellationToken = default);
    Task<InventoryTransaction?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
