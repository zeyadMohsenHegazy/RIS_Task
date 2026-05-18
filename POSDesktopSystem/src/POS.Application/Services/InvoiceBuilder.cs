using POS.Application.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;

namespace POS.Application.Services;

public static class InvoiceBuilder
{
    public static Invoice Build(CheckoutRequest request, InvoiceCalculationDto calculation, int userId)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(calculation);

        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("Cannot build an invoice without items.");
        }

        if (userId <= 0)
        {
            throw new ArgumentException("User id is required.", nameof(userId));
        }

        var timestamp = DateTime.Now;

        var invoice = new Invoice
        {
            UserId = userId,
            Date = timestamp,
            CreatedAt = timestamp,
            TotalAmount = calculation.TotalAmount,
            Discount = calculation.Discount,
            Tax = calculation.Tax,
            FinalAmount = calculation.FinalAmount,
            PaymentMethod = request.PaymentMethod,
            Items = request.Items.Select(item => new InvoiceItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                SubTotal = item.SubTotal
            }).ToList()
        };

        return invoice;
    }
}
