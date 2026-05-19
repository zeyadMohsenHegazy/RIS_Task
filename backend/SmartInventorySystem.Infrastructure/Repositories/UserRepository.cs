using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Domain.Entities;
using SmartInventorySystem.Infrastructure.Extensions;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure.Repositories;

public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<PagedResult<ApplicationUser>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ApplicationUser> query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Username.Contains(search));
        }

        query = query.OrderBy(u => u.Username);

        return await query.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(
        string username,
        int? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(u => u.Username == username);

        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasInventoryTransactionsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.InventoryTransactions
            .AnyAsync(t => t.CreatedByUserId == userId, cancellationToken);
    }

    public async Task<int> CountByRoleAsync(
        string role,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .CountAsync(u => u.Role == role, cancellationToken);
    }
}
