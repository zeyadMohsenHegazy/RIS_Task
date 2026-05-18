using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IInventoryRepository : IGenericRepository<InventoryTransaction>
{
    Task<PagedResult<InventoryTransaction>> GetHistoryPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);
    Task<InventoryTransaction?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
