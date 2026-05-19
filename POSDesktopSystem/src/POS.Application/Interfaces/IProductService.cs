using POS.Application.DTOs;

namespace POS.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<ProductDto>> CreateAsync(ProductUpsertDto product, CancellationToken cancellationToken = default);
    Task<ServiceResult<ProductDto>> UpdateAsync(int id, ProductUpsertDto product, CancellationToken cancellationToken = default);
    Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
