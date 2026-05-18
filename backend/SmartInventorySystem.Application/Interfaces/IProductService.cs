using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Products;

namespace SmartInventorySystem.Application.Interfaces;

public interface IProductService
{
    Task<PagedResponse<ProductDto>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
