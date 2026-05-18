using SmartInventorySystem.Application.DTOs.Warehouses;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Mappings;

namespace SmartInventorySystem.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<IReadOnlyList<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _warehouseRepository.GetAllOrderedAsync(cancellationToken);
        return WarehouseMapper.ToDtoList(warehouses);
    }

    public async Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken = default)
    {
        var warehouse = WarehouseMapper.ToEntity(dto);
        await _warehouseRepository.AddAsync(warehouse, cancellationToken);
        await _warehouseRepository.SaveChangesAsync(cancellationToken);
        return WarehouseMapper.ToDto(warehouse);
    }
}
