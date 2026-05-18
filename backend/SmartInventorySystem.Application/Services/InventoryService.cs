using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Inventory;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Mappings;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Domain.Enums;

namespace SmartInventorySystem.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public InventoryService(
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public Task<InventoryTransactionDto> StockInAsync(
        InventoryMovementDto dto,
        CancellationToken cancellationToken = default)
    {
        return ProcessMovementAsync(dto, TransactionType.In, cancellationToken);
    }

    public Task<InventoryTransactionDto> StockOutAsync(
        InventoryMovementDto dto,
        CancellationToken cancellationToken = default)
    {
        return ProcessMovementAsync(dto, TransactionType.Out, cancellationToken);
    }

    public async Task<PagedResponse<InventoryTransactionDto>> GetHistoryAsync(
        InventoryHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = query.Normalize();
        var search = query.NormalizedSearch();

        var result = await _inventoryRepository.GetHistoryPagedAsync(
            pageNumber,
            pageSize,
            search,
            query.TransactionType,
            cancellationToken);

        return PagedMapper.ToPagedResponse(result, InventoryMapper.ToDto);
    }

    private async Task<InventoryTransactionDto> ProcessMovementAsync(
        InventoryMovementDto dto,
        TransactionType transactionType,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var product = await _productRepository.GetByIdForUpdateAsync(dto.ProductId, cancellationToken)
            ?? throw new InvalidOperationException($"Product with id {dto.ProductId} was not found.");

        if (transactionType == TransactionType.Out && product.Quantity < dto.Quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {product.Quantity}, requested: {dto.Quantity}.");
        }

        product.Quantity = transactionType == TransactionType.In
            ? product.Quantity + dto.Quantity
            : product.Quantity - dto.Quantity;

        _productRepository.Update(product);

        var transaction = new InventoryTransaction(
            dto.ProductId,
            dto.Quantity,
            transactionType,
            userId);

        await _inventoryRepository.AddAsync(transaction, cancellationToken);
        await _inventoryRepository.SaveChangesAsync(cancellationToken);

        var saved = await _inventoryRepository.GetByIdWithDetailsAsync(transaction.Id, cancellationToken)
            ?? transaction;

        _cacheService.InvalidateProducts();
        return InventoryMapper.ToDto(saved);
    }
}
