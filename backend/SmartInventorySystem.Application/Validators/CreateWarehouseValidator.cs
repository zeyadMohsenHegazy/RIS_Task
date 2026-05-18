using FluentValidation;
using SmartInventorySystem.Application.DTOs.Warehouses;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.Application.Validators;

public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseDto>
{
    public CreateWarehouseValidator(IWarehouseRepository warehouseRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(async (name, cancellation) =>
                !await warehouseRepository.ExistsByNameAsync(name, cancellation))
            .WithMessage("A warehouse with this name already exists.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(500);
    }
}
