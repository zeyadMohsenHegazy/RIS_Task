using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Extensions;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Repositories;

public class InventoryRepository : GenericRepository<InventoryTransaction>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<PagedResult<InventoryTransaction>> GetHistoryPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<InventoryTransaction> query = DbSet
            .AsNoTracking()
            .Include(t => t.Product)
            .Include(t => t.CreatedByUser);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Product != null && t.Product.Name.Contains(search));
        }

        query = query
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.Id);

        return await query.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<InventoryTransaction?> GetByIdWithDetailsAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(t => t.Product)
            .Include(t => t.CreatedByUser)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}
