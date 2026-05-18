using POS.Application.DTOs;

namespace POS.Application.Services;

public static class InvoiceCalculator
{
    public static InvoiceCalculationDto Calculate(
        IEnumerable<PosCartItemDto> items,
        decimal discount,
        decimal tax)
    {
        ArgumentNullException.ThrowIfNull(items);

        var totalAmount = items.Sum(item => item.SubTotal);
        var normalizedDiscount = Math.Max(0, discount);
        var normalizedTax = Math.Max(0, tax);

        if (normalizedDiscount > totalAmount)
        {
            normalizedDiscount = totalAmount;
        }

        var finalAmount = Math.Max(0, totalAmount - normalizedDiscount + normalizedTax);

        return new InvoiceCalculationDto
        {
            TotalAmount = totalAmount,
            Discount = normalizedDiscount,
            Tax = normalizedTax,
            FinalAmount = finalAmount
        };
    }
}
