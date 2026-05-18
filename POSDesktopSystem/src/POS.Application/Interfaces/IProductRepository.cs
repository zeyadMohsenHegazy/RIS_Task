using POS.Domain.Entities;

namespace POS.Application.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
    Task<bool> BarcodeExistsAsync(string barcode, int? excludeProductId = null, CancellationToken cancellationToken = default);
    Task<bool> HasInvoiceItemsAsync(int productId, CancellationToken cancellationToken = default);
}
