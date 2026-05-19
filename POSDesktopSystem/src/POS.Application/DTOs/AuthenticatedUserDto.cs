namespace POS.Application.DTOs;

public sealed class AuthenticatedUserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string RoleDisplayName { get; init; } = string.Empty;
    public bool IsManager { get; init; }
    public bool IsCashier { get; init; }
}
