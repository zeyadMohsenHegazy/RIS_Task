namespace POS.Application.Validators;

public sealed class PosCheckoutValidator
{
    public ValidationResult ValidateSearchTerm(string? searchTerm) =>
        ValidationHelper.Required(searchTerm, "Barcode or product search term");

    public ValidationResult ValidateQuantity(int quantity) =>
        ValidationHelper.GreaterThanZero(quantity, "Quantity");

    public ValidationResult ValidateCartQuantity(int quantity) =>
        ValidationHelper.GreaterThanZero(quantity, "Cart quantity");

    public ValidationResult ValidateDiscount(decimal discount) =>
        ValidationHelper.NonNegative(discount, "Discount");

    public ValidationResult ValidateTax(decimal tax) =>
        ValidationHelper.NonNegative(tax, "Tax");
}
