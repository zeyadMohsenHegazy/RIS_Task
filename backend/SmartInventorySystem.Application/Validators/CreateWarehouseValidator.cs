using FluentValidation;
using SmartInventorySystem.Application.DTOs.Warehouses;

namespace SmartInventorySystem.Application.Validators;

public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseDto>
{
    public CreateWarehouseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(500);
    }
}
