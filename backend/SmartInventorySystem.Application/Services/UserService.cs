using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Users;
using SmartInventorySystem.Application.Interfaces;
using SmartInventorySystem.Application.Mappings;
using SmartInventorySystem.Domain.Constants;

namespace SmartInventorySystem.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResponse<UserDto>> GetPagedAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = query.Normalize();
        var search = query.NormalizedSearch();

        var result = await _userRepository.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            cancellationToken);

        return PagedMapper.ToPagedResponse(result, UserMapper.ToDto);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : UserMapper.ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.UsernameExistsAsync(dto.Username, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Username must be unique.");
        }

        var passwordHash = _passwordHasher.HashPassword(dto.Password);
        var user = UserMapper.ToEntity(dto, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return UserMapper.ToDto(user);
    }

    public async Task<UserDto?> UpdateAsync(
        int id,
        UpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (await _userRepository.UsernameExistsAsync(dto.Username, id, cancellationToken))
        {
            throw new InvalidOperationException("Username must be unique.");
        }

        if (user.Role == Roles.Admin
            && dto.Role != Roles.Admin
            && await _userRepository.CountByRoleAsync(Roles.Admin, cancellationToken) <= 1)
        {
            throw new InvalidOperationException("Cannot demote the last admin user.");
        }

        string? passwordHash = string.IsNullOrWhiteSpace(dto.Password)
            ? null
            : _passwordHasher.HashPassword(dto.Password);

        UserMapper.ApplyUpdate(user, dto, passwordHash);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return UserMapper.ToDto(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        if (await _userRepository.HasInventoryTransactionsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(
                "Cannot delete a user that has inventory transactions.");
        }

        if (user.Role == Roles.Admin
            && await _userRepository.CountByRoleAsync(Roles.Admin, cancellationToken) <= 1)
        {
            throw new InvalidOperationException("Cannot delete the last admin user.");
        }

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
