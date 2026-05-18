using SmartInventorySystem.Application.DTOs.Warehouses;

namespace SmartInventorySystem.Application.Interfaces;

public interface IWarehouseService
{
    Task<IReadOnlyList<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto, CancellationToken cancellationToken = default);
}
