using SmartInventorySystem.Application.DTOs.Warehouses;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Mappings;

public static class WarehouseMapper
{
    public static WarehouseDto ToDto(Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location
        };
    }

    public static IReadOnlyList<WarehouseDto> ToDtoList(IEnumerable<Warehouse> warehouses)
    {
        return warehouses.Select(ToDto).ToList();
    }

    public static Warehouse ToEntity(CreateWarehouseDto dto)
    {
        return new Warehouse(dto.Name, dto.Location);
    }
}
