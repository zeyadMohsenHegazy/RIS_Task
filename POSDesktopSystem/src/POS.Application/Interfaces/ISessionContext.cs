using POS.Application.DTOs;

namespace POS.Application.Interfaces;

public interface ISessionContext
{
    AuthenticatedUserDto? CurrentUser { get; }
    bool IsAuthenticated { get; }
    void SetUser(AuthenticatedUserDto user);
    void Clear();
}