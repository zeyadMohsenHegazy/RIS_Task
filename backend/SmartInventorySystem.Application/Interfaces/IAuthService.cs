using SmartInventorySystem.Application.DTOs.Auth;

namespace SmartInventorySystem.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
