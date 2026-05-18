using FluentValidation;
using SmartInventorySystem.Application.DTOs.Inventory;

namespace SmartInventorySystem.Application.Validators;

public class InventoryMovementValidator : AbstractValidator<InventoryMovementDto>
{
    public InventoryMovementValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
