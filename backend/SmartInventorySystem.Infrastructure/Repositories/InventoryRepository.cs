using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Repositories;

public class InventoryRepository : GenericRepository<InventoryTransaction>, IInventoryRepository
{
    public InventoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<InventoryTransaction>> GetHistoryAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(t => t.Product)
            .Include(t => t.CreatedByUser)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.Id)
            .ToListAsync(cancellationToken);
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
