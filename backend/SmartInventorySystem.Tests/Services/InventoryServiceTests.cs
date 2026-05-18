using FluentAssertions;
using Moq;
using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Inventory;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Services;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Domain.Enums;

namespace SmartInventorySystem.Tests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();
    private readonly InventoryService _sut;

    public InventoryServiceTests()
    {
        _currentUserServiceMock.Setup(u => u.UserId).Returns(1);
        _currentUserServiceMock.Setup(u => u.Username).Returns("admin");

        _sut = new InventoryService(
            _productRepositoryMock.Object,
            _inventoryRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task StockInAsync_IncreasesProductQuantity()
    {
        var product = CreateProduct(quantity: 10);
        SetupProductForUpdate(product);
        SetupTransactionPersistence();

        var dto = new InventoryMovementDto { ProductId = 1, Quantity = 5 };

        await _sut.StockInAsync(dto);

        product.Quantity.Should().Be(15);
        _productRepositoryMock.Verify(r => r.Update(product), Times.Once);
        _inventoryRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(c => c.InvalidateProducts(), Times.Once);
    }

    [Fact]
    public async Task StockOutAsync_DecreasesProductQuantity()
    {
        var product = CreateProduct(quantity: 20);
        SetupProductForUpdate(product);
        SetupTransactionPersistence();

        var dto = new InventoryMovementDto { ProductId = 1, Quantity = 8 };

        await _sut.StockOutAsync(dto);

        product.Quantity.Should().Be(12);
    }

    [Fact]
    public async Task StockOutAsync_WhenInsufficientStock_ThrowsInvalidOperationException()
    {
        var product = CreateProduct(quantity: 3);
        _productRepositoryMock
            .Setup(r => r.GetByIdForUpdateAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = new InventoryMovementDto { ProductId = 1, Quantity = 10 };

        var act = () => _sut.StockOutAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Insufficient stock. Available: 3, requested: 10.");
    }

    [Fact]
    public async Task StockInAsync_WhenProductNotFound_ThrowsInvalidOperationException()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdForUpdateAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var dto = new InventoryMovementDto { ProductId = 99, Quantity = 5 };

        var act = () => _sut.StockInAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Product with id 99 was not found.");
    }

    [Fact]
    public async Task StockInAsync_WhenUserNotAuthenticated_ThrowsUnauthorizedAccessException()
    {
        _currentUserServiceMock.Setup(u => u.UserId).Returns((int?)null);

        var act = () => _sut.StockInAsync(new InventoryMovementDto { ProductId = 1, Quantity = 1 });

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not authenticated.");
    }

    [Fact]
    public async Task StockOutAsync_WhenQuantityEqualsStock_SucceedsWithZeroRemaining()
    {
        var product = CreateProduct(quantity: 10);
        SetupProductForUpdate(product);
        SetupTransactionPersistence();

        await _sut.StockOutAsync(new InventoryMovementDto { ProductId = 1, Quantity = 10 });

        product.Quantity.Should().Be(0);
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsPagedHistory()
    {
        var product = CreateProduct();
        var transactions = new List<InventoryTransaction>
        {
            new(1, 5, TransactionType.In, 1) { Id = 1, Product = product },
            new(1, 2, TransactionType.Out, 1) { Id = 2, Product = product }
        };
        var paged = new PagedResult<InventoryTransaction>(transactions, 1, 10, 2);

        _inventoryRepositoryMock
            .Setup(r => r.GetHistoryPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        var result = await _sut.GetHistoryAsync(new InventoryHistoryQuery { PageNumber = 1, PageSize = 10 });

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Items[0].TransactionType.Should().Be(TransactionType.In);
    }

    private void SetupProductForUpdate(Product product)
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdForUpdateAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
    }

    private void SetupTransactionPersistence()
    {
        _inventoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>(), It.IsAny<CancellationToken>()))
            .Callback<InventoryTransaction, CancellationToken>((t, _) => t.Id = 100)
            .ReturnsAsync((InventoryTransaction t, CancellationToken _) => t);

        _inventoryRepositoryMock
            .Setup(r => r.GetByIdWithDetailsAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) =>
            {
                var product = CreateProduct();
                return new InventoryTransaction(1, 5, TransactionType.In, 1)
                {
                    Id = id,
                    Product = product,
                    CreatedByUser = new ApplicationUser("admin", "hash", "Admin")
                };
            });
    }

    private static Product CreateProduct(int id = 1, int quantity = 10)
    {
        var product = new Product("Laptop", "LAP-001", 999.99m, quantity, 1);
        product.Id = id;
        return product;
    }
}
