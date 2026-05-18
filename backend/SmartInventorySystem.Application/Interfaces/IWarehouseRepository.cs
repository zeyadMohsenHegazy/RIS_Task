using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IWarehouseRepository : IGenericRepository<Warehouse>
{
    Task<IReadOnlyList<Warehouse>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
