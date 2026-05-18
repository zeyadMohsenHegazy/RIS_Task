using FluentValidation;
using SmartInventorySystem.Application.DTOs.Inventory;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.Application.Validators;

public class InventoryMovementValidator : AbstractValidator<InventoryMovementDto>
{
    public InventoryMovementValidator(IProductRepository productRepository)
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .MustAsync(async (productId, cancellation) =>
                await productRepository.GetByIdAsync(productId, cancellation) is not null)
            .WithMessage("Product does not exist.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
