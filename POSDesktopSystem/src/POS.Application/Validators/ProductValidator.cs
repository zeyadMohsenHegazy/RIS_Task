using POS.Application.DTOs;

namespace POS.Application.Validators;

public sealed class ProductValidator
{
    public ValidationResult Validate(ProductUpsertDto product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return ValidationHelper.Combine(
            ValidationHelper.Required(product.Name, "Product name"),
            ValidationHelper.MaxLength(product.Name, 200, "Product name"),
            ValidationHelper.Required(product.Barcode, "Barcode"),
            ValidationHelper.MaxLength(product.Barcode, 50, "Barcode"),
            ValidationHelper.NonNegative(product.Price, "Price"),
            ValidationHelper.NonNegative(product.StockQuantity, "Stock quantity"));
    }
}
