using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IInventoryRepository : IGenericRepository<InventoryTransaction>
{
    Task<IReadOnlyList<InventoryTransaction>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task<InventoryTransaction?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
}
