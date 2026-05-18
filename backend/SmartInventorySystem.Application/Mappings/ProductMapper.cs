using SmartInventorySystem.Application.DTOs.Products;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Mappings;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Price = product.Price,
            Quantity = product.Quantity,
            WarehouseId = product.WarehouseId,
            WarehouseName = product.Warehouse?.Name,
            CreatedAt = product.CreatedAt
        };
    }

    public static IReadOnlyList<ProductDto> ToDtoList(IEnumerable<Product> products)
    {
        return products.Select(ToDto).ToList();
    }

    public static Product ToEntity(CreateProductDto dto)
    {
        return new Product(dto.Name, dto.SKU, dto.Price, dto.Quantity, dto.WarehouseId);
    }

    public static void ApplyUpdate(Product product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.SKU = dto.SKU;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.WarehouseId = dto.WarehouseId;
    }
}
