using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Products;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Mappings;

namespace SmartInventorySystem.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly ICacheService _cacheService;

    public ProductService(
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _cacheService = cacheService;
    }

    public Task<PagedResponse<ProductDto>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = _cacheService.BuildProductsCacheKey(query);

        return _cacheService.GetOrSetAsync(
            cacheKey,
            ct => LoadProductsPageAsync(query, ct),
            cancellationToken);
    }

    private async Task<PagedResponse<ProductDto>> LoadProductsPageAsync(
        PaginationQuery query,
        CancellationToken cancellationToken)
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
        if (await _productRepository.GetBySkuAsync(dto.SKU, cancellationToken) is not null)
        {
            throw new InvalidOperationException("SKU must be unique.");
        }

        if (await _warehouseRepository.GetByIdAsync(dto.WarehouseId, cancellationToken) is null)
        {
            throw new InvalidOperationException("Warehouse does not exist.");
        }

        var product = ProductMapper.ToEntity(dto);
        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var created = await _productRepository.GetByIdWithWarehouseAsync(product.Id, cancellationToken)
            ?? product;

        _cacheService.InvalidateProducts();
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

        if (await _warehouseRepository.GetByIdAsync(dto.WarehouseId, cancellationToken) is null)
        {
            throw new InvalidOperationException("Warehouse does not exist.");
        }

        ProductMapper.ApplyUpdate(product, dto);
        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var updated = await _productRepository.GetByIdWithWarehouseAsync(id, cancellationToken)
            ?? product;

        _cacheService.InvalidateProducts();
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
        _cacheService.InvalidateProducts();
        return true;
    }
}
