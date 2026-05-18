using FluentValidation;
using SmartInventorySystem.Application.DTOs.Products;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator(
        IWarehouseRepository warehouseRepository,
        IProductRepository productRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (sku, cancellation) =>
                await productRepository.GetBySkuAsync(sku, cancellation) is null)
            .WithMessage("SKU must be unique.");

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.WarehouseId)
            .GreaterThan(0)
            .MustAsync(async (warehouseId, cancellation) =>
                await warehouseRepository.GetByIdAsync(warehouseId, cancellation) is not null)
            .WithMessage("Warehouse does not exist.");
    }
}
