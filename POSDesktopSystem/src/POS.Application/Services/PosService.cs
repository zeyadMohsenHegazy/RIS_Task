using POS.Application.DTOs;
using POS.Application.Helpers;
using POS.Application.Interfaces;
using POS.Domain.Enums;

namespace POS.Application.Services;

public sealed class PosService : IPosService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceService _invoiceService;
    private readonly ISessionContext _sessionContext;
    private readonly PosCart _cart;

    public PosService(
        IUnitOfWork unitOfWork,
        IInvoiceService invoiceService,
        ISessionContext sessionContext,
        PosCart cart)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        _sessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
        _cart = cart ?? throw new ArgumentNullException(nameof(cart));
    }

    public IReadOnlyList<PosCartItemDto> CartItems => _cart.Items;

    public async Task<ServiceResult<ProductDto>> LookupProductAsync(
        string barcodeOrSearch,
        CancellationToken cancellationToken = default)
    {
        var searchResult = await SearchProductsAsync(barcodeOrSearch, cancellationToken);
        if (!searchResult.IsSuccess || searchResult.Data is null)
        {
            return ServiceResult<ProductDto>.Failure(searchResult.Message);
        }

        if (searchResult.Data.Products.Count != 1)
        {
            return ServiceResult<ProductDto>.Failure(
                searchResult.Data.Products.Count == 0
                    ? "Product not found."
                    : "Multiple products matched. Select one from the list.");
        }

        return ServiceResult<ProductDto>.Success(searchResult.Data.Products[0]);
    }

    public async Task<ServiceResult<ProductLookupResult>> SearchProductsAsync(
        string barcodeOrSearch,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcodeOrSearch))
        {
            return ServiceResult<ProductLookupResult>.Failure("Enter a barcode or product search term.");
        }

        var term = barcodeOrSearch.Trim();
        var exactMatch = await _unitOfWork.Products.GetByBarcodeAsync(term, cancellationToken);
        if (exactMatch is not null)
        {
            if (exactMatch.StockQuantity <= 0)
            {
                return ServiceResult<ProductLookupResult>.Failure($"'{exactMatch.Name}' is out of stock.");
            }

            return ServiceResult<ProductLookupResult>.Success(new ProductLookupResult
            {
                IsExactBarcodeMatch = true,
                Products = [MapProduct(exactMatch)]
            });
        }

        IReadOnlyList<Domain.Entities.Product> matches;
        if (BarcodeNormalizer.LooksLikeBarcode(term))
        {
            matches = await _unitOfWork.Products.SearchByBarcodeAsync(term, cancellationToken);
        }
        else
        {
            matches = await _unitOfWork.Products.SearchAsync(term, cancellationToken);
        }

        var inStockProducts = matches
            .Where(product => product.StockQuantity > 0)
            .Select(MapProduct)
            .ToList();

        if (inStockProducts.Count == 0)
        {
            return ServiceResult<ProductLookupResult>.Failure("Product not found or out of stock.");
        }

        return ServiceResult<ProductLookupResult>.Success(new ProductLookupResult
        {
            IsExactBarcodeMatch = false,
            Products = inStockProducts
        });
    }

    public ServiceResult AddToCart(ProductDto product, int quantity) =>
        _cart.AddItem(product, quantity);

    public async Task<ServiceResult> UpdateCartItemQuantityAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return ServiceResult.Failure("Product no longer exists.");
        }

        var cartItem = _cart.Items.FirstOrDefault(item => item.ProductId == productId);
        if (cartItem is not null)
        {
            cartItem.AvailableStock = product.StockQuantity;
            cartItem.UnitPrice = product.Price;
        }

        return _cart.UpdateQuantity(productId, quantity);
    }

    public ServiceResult RemoveFromCart(int productId) =>
        _cart.RemoveItem(productId);

    public void ClearCart() => _cart.Clear();

    public InvoiceCalculationDto CalculateTotals(decimal discount, decimal tax) =>
        InvoiceCalculator.Calculate(_cart.Items, discount, tax);

    public async Task<ServiceResult<int>> CheckoutAsync(
        decimal discount,
        decimal tax,
        PaymentMethod paymentMethod,
        CancellationToken cancellationToken = default)
    {
        if (_cart.Items.Count == 0)
        {
            return ServiceResult<int>.Failure("Cart is empty. Add products before checkout.");
        }

        var currentUser = _sessionContext.CurrentUser;
        if (currentUser is null)
        {
            return ServiceResult<int>.Failure("You must be logged in to complete checkout.");
        }

        var calculation = CalculateTotals(discount, tax);
        if (calculation.FinalAmount <= 0 && calculation.TotalAmount > 0)
        {
            return ServiceResult<int>.Failure("Final amount must be greater than zero.");
        }

        var productIds = _cart.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await _unitOfWork.Products.GetByIdsAsync(productIds, cancellationToken);
        var productMap = products.ToDictionary(product => product.Id);

        foreach (var cartItem in _cart.Items)
        {
            if (!productMap.TryGetValue(cartItem.ProductId, out var product))
            {
                return ServiceResult<int>.Failure($"Product '{cartItem.ProductName}' no longer exists.");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                return ServiceResult<int>.Failure(
                    $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}.");
            }
        }

        var checkoutRequest = new CheckoutRequest
        {
            Discount = calculation.Discount,
            Tax = calculation.Tax,
            PaymentMethod = paymentMethod,
            Items = _cart.Items.ToList()
        };

        var stageResult = await _invoiceService.StageInvoiceAsync(
            checkoutRequest,
            calculation,
            currentUser.Id,
            cancellationToken);

        if (!stageResult.IsSuccess || stageResult.Data is null)
        {
            return ServiceResult<int>.Failure(stageResult.Message);
        }

        foreach (var cartItem in _cart.Items)
        {
            var product = productMap[cartItem.ProductId];
            product.StockQuantity -= cartItem.Quantity;
            _unitOfWork.Products.Update(product);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var invoice = stageResult.Data;
        _invoiceService.LogCreatedInvoice(invoice, currentUser.Id, currentUser.Username, checkoutRequest.Items.Count);
        _cart.Clear();

        return ServiceResult<int>.Success(invoice.Id, $"Sale completed. Invoice #{invoice.Id}.");
    }

    private static ProductDto MapProduct(Domain.Entities.Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };
}
