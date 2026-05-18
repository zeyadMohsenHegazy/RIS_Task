using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IUserRepository : IGenericRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
