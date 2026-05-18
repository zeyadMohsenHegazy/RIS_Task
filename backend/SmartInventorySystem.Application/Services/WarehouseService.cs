using SmartInventorySystem.Application.DTOs.Warehouses;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Mappings;

namespace SmartInventorySystem.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICacheService _cacheService;

    public WarehouseService(IWarehouseRepository warehouseRepository, ICacheService cacheService)
    {
        _warehouseRepository = warehouseRepository;
        _cacheService = cacheService;
    }

    public Task<IReadOnlyList<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = _cacheService.BuildWarehousesCacheKey();

        return _cacheService.GetOrSetAsync(
            cacheKey,
            LoadWarehousesAsync,
            cancellationToken);
    }

    private async Task<IReadOnlyList<WarehouseDto>> LoadWarehousesAsync(
        CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseRepository.GetAllOrderedAsync(cancellationToken);
        return WarehouseMapper.ToDtoList(warehouses);
    }

    public async Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken = default)
    {
        if (await _warehouseRepository.ExistsByNameAsync(dto.Name, cancellationToken))
        {
            throw new InvalidOperationException("A warehouse with this name already exists.");
        }

        var warehouse = WarehouseMapper.ToEntity(dto);
        await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);
        _cacheService.InvalidateWarehouses();
        return WarehouseMapper.ToDto(warehouse);
    }
}
