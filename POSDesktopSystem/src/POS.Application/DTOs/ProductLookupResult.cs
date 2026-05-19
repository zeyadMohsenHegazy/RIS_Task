namespace POS.Application.DTOs;

public sealed class ProductLookupResult
{
    public bool IsExactBarcodeMatch { get; init; }

    public IReadOnlyList<ProductDto> Products { get; init; } = Array.Empty<ProductDto>();
}
