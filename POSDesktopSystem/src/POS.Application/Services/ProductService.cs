using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Validators;
using POS.Domain.Entities;

namespace POS.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductValidator _productValidator;

    public ProductService(IUnitOfWork unitOfWork, ProductValidator productValidator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
    }

    public async Task<IReadOnlyList<ProductDto>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.SearchAsync(searchTerm, cancellationToken);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(
        ProductUpsertDto product,
        CancellationToken cancellationToken = default)
    {
        var validation = _productValidator.Validate(product);
        if (!validation.IsValid)
        {
            return ServiceResult<ProductDto>.Failure(validation.ErrorMessage);
        }

        var normalizedBarcode = product.Barcode.Trim();
        if (await _unitOfWork.Products.BarcodeExistsAsync(normalizedBarcode, cancellationToken: cancellationToken))
        {
            return ServiceResult<ProductDto>.Failure("A product with this barcode already exists.");
        }

        var entity = new Product
        {
            Name = product.Name.Trim(),
            Barcode = normalizedBarcode,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };

        await _unitOfWork.Products.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<ProductDto>.Success(MapToDto(entity), "Product added successfully.");
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(
        int id,
        ProductUpsertDto product,
        CancellationToken cancellationToken = default)
    {
        var validation = _productValidator.Validate(product);
        if (!validation.IsValid)
        {
            return ServiceResult<ProductDto>.Failure(validation.ErrorMessage);
        }

        var entity = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ServiceResult<ProductDto>.Failure("Product not found.");
        }

        var normalizedBarcode = product.Barcode.Trim();
        if (await _unitOfWork.Products.BarcodeExistsAsync(normalizedBarcode, id, cancellationToken))
        {
            return ServiceResult<ProductDto>.Failure("A product with this barcode already exists.");
        }

        entity.Name = product.Name.Trim();
        entity.Barcode = normalizedBarcode;
        entity.Price = product.Price;
        entity.StockQuantity = product.StockQuantity;

        _unitOfWork.Products.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<ProductDto>.Success(MapToDto(entity), "Product updated successfully.");
    }

    public async Task<ServiceResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return ServiceResult.Failure("Product not found.");
        }

        if (await _unitOfWork.Products.HasInvoiceItemsAsync(id, cancellationToken))
        {
            return ServiceResult.Failure("Cannot delete this product because it is used in existing invoices.");
        }

        _unitOfWork.Products.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success("Product deleted successfully.");
    }

    private static ProductDto MapToDto(Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Barcode = product.Barcode,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };
}
