using SmartInventorySystem.Application.DTOs.Common;

namespace SmartInventorySystem.Application.Interfaces;

public interface ICacheService
{
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default);

    string BuildProductsCacheKey(PaginationQuery query);

    string BuildWarehousesCacheKey();

    void InvalidateProducts();

    void InvalidateWarehouses();
}
