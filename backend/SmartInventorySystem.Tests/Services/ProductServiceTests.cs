using FluentAssertions;
using Moq;
using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Products;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Services;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IWarehouseRepository> _warehouseRepositoryMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        SetupCachePassthrough();
        SetupWarehouseExists();
        _sut = new ProductService(
            _productRepositoryMock.Object,
            _warehouseRepositoryMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProductDto()
    {
        var product = CreateProduct(id: 1);
        _productRepositoryMock
            .Setup(r => r.GetByIdWithWarehouseAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _sut.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Laptop");
        result.SKU.Should().Be("LAP-001");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductNotFound_ReturnsNull()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdWithWarehouseAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedProducts()
    {
        var products = new List<Product> { CreateProduct(id: 1), CreateProduct(id: 2, name: "Mouse", sku: "MOU-001") };
        var pagedResult = new PagedResult<Product>(products, 1, 10, 2);

        _productRepositoryMock
            .Setup(r => r.GetPagedWithWarehouseAsync(1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _sut.GetPagedAsync(new PaginationQuery { PageNumber = 1, PageSize = 10 });

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_CreatesProductAndInvalidatesCache()
    {
        var dto = new CreateProductDto
        {
            Name = "Keyboard",
            SKU = "KEY-001",
            Price = 49.99m,
            Quantity = 25,
            WarehouseId = 1
        };

        _productRepositoryMock
            .Setup(r => r.GetBySkuAsync(dto.SKU, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => p.Id = 5)
            .ReturnsAsync((Product p, CancellationToken _) => p);

        _productRepositoryMock
            .Setup(r => r.GetByIdWithWarehouseAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => CreateProduct(id: id, name: dto.Name, sku: dto.SKU));

        var result = await _sut.CreateAsync(dto);

        result.Id.Should().Be(5);
        result.Name.Should().Be("Keyboard");
        _productRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.InvalidateProducts(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductNotFound_ReturnsNull()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _sut.UpdateAsync(10, new UpdateProductDto());

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenDuplicateSku_ThrowsInvalidOperationException()
    {
        var existing = CreateProduct(id: 1, sku: "LAP-001");
        var other = CreateProduct(id: 2, sku: "OTHER-SKU");

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _productRepositoryMock
            .Setup(r => r.GetBySkuAsync("OTHER-SKU", It.IsAny<CancellationToken>()))
            .ReturnsAsync(other);

        var act = () => _sut.UpdateAsync(1, new UpdateProductDto { SKU = "OTHER-SKU" });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SKU must be unique.");
    }

    [Fact]
    public async Task UpdateAsync_WhenValid_UpdatesProduct()
    {
        var product = CreateProduct(id: 1);
        var dto = new UpdateProductDto
        {
            Name = "Laptop Pro",
            SKU = "LAP-001",
            Price = 1299.99m,
            Quantity = 15,
            WarehouseId = 1
        };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.GetBySkuAsync(dto.SKU, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _productRepositoryMock
            .Setup(r => r.GetByIdWithWarehouseAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _sut.UpdateAsync(1, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Laptop Pro");
        result.Price.Should().Be(1299.99m);
        _cacheServiceMock.Verify(c => c.InvalidateProducts(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductNotFound_ReturnsFalse()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _sut.DeleteAsync(5);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenHasInventoryTransactions_ThrowsInvalidOperationException()
    {
        var product = CreateProduct(id: 1);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.HasInventoryTransactionsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var act = () => _sut.DeleteAsync(1);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot delete a product that has inventory transactions.");
    }

    [Fact]
    public async Task DeleteAsync_WhenValid_DeletesAndInvalidatesCache()
    {
        var product = CreateProduct(id: 1);
        _productRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.HasInventoryTransactionsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _sut.DeleteAsync(1);

        result.Should().BeTrue();
        _productRepositoryMock.Verify(r => r.Delete(product), Times.Once);
        _cacheServiceMock.Verify(c => c.InvalidateProducts(), Times.Once);
    }

    private void SetupWarehouseExists()
    {
        _warehouseRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Warehouse("Main", "Cairo"));
    }

    private void SetupCachePassthrough()
    {
        _cacheServiceMock
            .Setup(c => c.BuildProductsCacheKey(It.IsAny<PaginationQuery>()))
            .Returns("products-test-key");

        _cacheServiceMock
            .Setup(c => c.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, Task<PagedResponse<ProductDto>>>>(),
                It.IsAny<CancellationToken>()))
            .Returns((string _, Func<CancellationToken, Task<PagedResponse<ProductDto>>> factory, CancellationToken ct) =>
                factory(ct));
    }

    private static Product CreateProduct(
        int id = 0,
        string name = "Laptop",
        string sku = "LAP-001",
        decimal price = 999.99m,
        int quantity = 10,
        int warehouseId = 1)
    {
        var product = new Product(name, sku, price, quantity, warehouseId);
        if (id > 0)
        {
            product.Id = id;
        }

        return product;
    }
}
