using SmartInventorySystem.Domain.Entities;

namespace SmartInventorySystem.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user);
}
