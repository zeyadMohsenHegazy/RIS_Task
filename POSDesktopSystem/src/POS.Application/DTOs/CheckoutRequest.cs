using POS.Domain.Enums;

namespace POS.Application.DTOs;

public sealed class CheckoutRequest
{
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public IReadOnlyList<PosCartItemDto> Items { get; init; } = Array.Empty<PosCartItemDto>();
}
