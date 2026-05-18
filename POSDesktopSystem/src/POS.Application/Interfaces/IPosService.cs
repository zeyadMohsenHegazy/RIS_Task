using POS.Application.DTOs;
using POS.Domain.Enums;

namespace POS.Application.Interfaces;

public interface IPosService
{
    IReadOnlyList<PosCartItemDto> CartItems { get; }
    Task<ServiceResult<ProductDto>> LookupProductAsync(string barcodeOrSearch, CancellationToken cancellationToken = default);
    Task<ServiceResult<ProductLookupResult>> SearchProductsAsync(string barcodeOrSearch, CancellationToken cancellationToken = default);
    ServiceResult AddToCart(ProductDto product, int quantity);
    Task<ServiceResult> UpdateCartItemQuantityAsync(int productId, int quantity, CancellationToken cancellationToken = default);
    ServiceResult RemoveFromCart(int productId);
    void ClearCart();
    InvoiceCalculationDto CalculateTotals(decimal discount, decimal tax);
    Task<ServiceResult<int>> CheckoutAsync(decimal discount, decimal tax, PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
}
