using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Products;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Mappings;

namespace SmartInventorySystem.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResponse<ProductDto>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = query.Normalize();
        var search = query.NormalizedSearch();

        var result = await _productRepository.GetPagedWithWarehouseAsync(
            pageNumber,
            pageSize,
            search,
            cancellationToken);

        return PagedMapper.ToPagedResponse(result, ProductMapper.ToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdWithWarehouseAsync(id, cancellationToken);
        return product is null ? null : ProductMapper.ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = ProductMapper.ToEntity(dto);
        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var created = await _productRepository.GetByIdWithWarehouseAsync(product.Id, cancellationToken)
            ?? product;

        return ProductMapper.ToDto(created);
    }

    public async Task<ProductDto?> UpdateAsync(
        int id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var duplicateSku = await _productRepository.GetBySkuAsync(dto.SKU, cancellationToken);
        if (duplicateSku is not null && duplicateSku.Id != id)
        {
            throw new InvalidOperationException("SKU must be unique.");
        }

        ProductMapper.ApplyUpdate(product, dto);
        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var updated = await _productRepository.GetByIdWithWarehouseAsync(id, cancellationToken)
            ?? product;

        return ProductMapper.ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        if (await _productRepository.HasInventoryTransactionsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Cannot delete a product that has inventory transactions.");
        }

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
