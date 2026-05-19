using FluentValidation;
using SmartInventorySystem.Application.DTOs.Users;
using SmartInventorySystem.Domain.Constants;

namespace SmartInventorySystem.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => role is Roles.Admin or Roles.Employee)
            .WithMessage($"Role must be '{Roles.Admin}' or '{Roles.Employee}'.");

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}
