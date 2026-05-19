namespace SmartInventorySystem.Application.DTOs.Users;

public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Password { get; set; }
}
