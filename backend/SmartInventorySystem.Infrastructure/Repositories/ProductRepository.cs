using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Extensions;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<PagedResult<Product>> GetPagedWithWarehouseAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = DbSet
            .AsNoTracking()
            .Include(p => p.Warehouse);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        query = query.OrderBy(p => p.Name);

        return await query.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<Product?> GetByIdWithWarehouseAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(p => p.Warehouse)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<bool> HasInventoryTransactionsAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .AnyAsync(t => t.ProductId == productId, cancellationToken);
    }

    public async Task<Product?> GetByIdForUpdateAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
