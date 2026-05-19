using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IUserRepository : IGenericRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<PagedResult<ApplicationUser>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(
        string username,
        int? excludeUserId = null,
        CancellationToken cancellationToken = default);
    Task<bool> HasInventoryTransactionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> CountByRoleAsync(string role, CancellationToken cancellationToken = default);
}
