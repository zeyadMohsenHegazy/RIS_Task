using POS.Application.DTOs;
using POS.Application.Interfaces;

namespace POS.UI.Session;

public sealed class SessionContext : ISessionContext
{
    public AuthenticatedUserDto? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public void SetUser(AuthenticatedUserDto user)
    {
        ArgumentNullException.ThrowIfNull(user);
        CurrentUser = user;
    }

    public void Clear()
    {
        CurrentUser = null;
    }
}
