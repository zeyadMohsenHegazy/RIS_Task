using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<Product>> GetAllWithWarehouseAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithWarehouseAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<bool> HasInventoryTransactionsAsync(int productId, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
}
