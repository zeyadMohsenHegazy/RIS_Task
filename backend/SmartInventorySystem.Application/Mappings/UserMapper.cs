using SmartInventorySystem.Application.DTOs.Users;
using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    public static ApplicationUser ToEntity(CreateUserDto dto, string passwordHash)
    {
        return new ApplicationUser(dto.Username, passwordHash, dto.Role);
    }

    public static void ApplyUpdate(ApplicationUser user, UpdateUserDto dto, string? passwordHash)
    {
        user.Username = dto.Username;
        user.Role = dto.Role;

        if (!string.IsNullOrWhiteSpace(passwordHash))
        {
            user.PasswordHash = passwordHash;
        }
    }
}
