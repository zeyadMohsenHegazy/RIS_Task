using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Validators;

namespace POS.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionContext _sessionContext;
    private readonly LoginValidator _loginValidator;
    private readonly IAppLogger _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ISessionContext sessionContext,
        LoginValidator loginValidator,
        IAppLogger logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _sessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
        _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<LoginResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedUsername = request.Username.Trim();

        var validation = _loginValidator.Validate(request.Username, request.Password);
        if (!validation.IsValid)
        {
            _logger.LogAction(
                "Login attempt failed",
                $"Username={normalizedUsername}; Reason={validation.ErrorMessage}");
            return LoginResult.Failure(validation.ErrorMessage);
        }

        var user = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogAction(
                "Login attempt failed",
                $"Username={normalizedUsername}; Reason=Invalid credentials");
            return LoginResult.Failure("Invalid username or password.");
        }

        var authenticatedUser = new AuthenticatedUserDto
        {
            Id = user.Id,
            Username = user.Username,
            RoleDisplayName = RolePermissions.GetRoleDisplayName(user.Role),
            IsManager = user.Role == Domain.Enums.UserRole.Manager,
            IsCashier = user.Role == Domain.Enums.UserRole.Cashier
        };

        _sessionContext.SetUser(authenticatedUser);
        _logger.LogAction(
            "Login successful",
            $"Username={user.Username}; Role={user.Role}; UserId={user.Id}");

        return LoginResult.Success(authenticatedUser);
    }

    public void Logout()
    {
        var username = _sessionContext.CurrentUser?.Username ?? "Unknown";
        _sessionContext.Clear();
        _logger.LogAction("Logout", $"Username={username}");
    }
}
