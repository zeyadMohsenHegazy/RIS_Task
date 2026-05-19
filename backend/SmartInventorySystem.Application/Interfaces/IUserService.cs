using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Users;

namespace SmartInventorySystem.Application.Interfaces;

public interface IUserService
{
    Task<PagedResponse<UserDto>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
