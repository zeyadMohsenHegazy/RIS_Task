using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Settings;

namespace SmartInventorySystem.Infrastructure.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _defaultExpiration;
    private long _productsCacheVersion;
    private long _warehousesCacheVersion;

    public MemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CacheSettings> cacheSettings)
    {
        _memoryCache = memoryCache;
        _defaultExpiration = TimeSpan.FromMinutes(cacheSettings.Value.DefaultExpirationMinutes);
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out T? cachedValue) && cachedValue is not null)
        {
            return cachedValue;
        }

        var value = await factory(cancellationToken);

        _memoryCache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _defaultExpiration
        });

        return value;
    }

    public string BuildProductsCacheKey(PaginationQuery query)
    {
        var (pageNumber, pageSize) = query.Normalize();
        var search = query.NormalizedSearch() ?? string.Empty;
        var version = Interlocked.Read(ref _productsCacheVersion);

        return $"products:v{version}:p{pageNumber}:s{pageSize}:q{search.ToLowerInvariant()}";
    }

    public string BuildWarehousesCacheKey()
    {
        var version = Interlocked.Read(ref _warehousesCacheVersion);
        return $"warehouses:v{version}";
    }

    public void InvalidateProducts()
    {
        Interlocked.Increment(ref _productsCacheVersion);
    }

    public void InvalidateWarehouses()
    {
        Interlocked.Increment(ref _warehousesCacheVersion);
    }
}
