namespace POS.Application.DTOs;

public sealed class LoginResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public AuthenticatedUserDto? User { get; init; }

    public static LoginResult Success(AuthenticatedUserDto user) =>
        new() { IsSuccess = true, Message = "Login successful.", User = user };

    public static LoginResult Failure(string message) =>
        new() { IsSuccess = false, Message = message };
}
