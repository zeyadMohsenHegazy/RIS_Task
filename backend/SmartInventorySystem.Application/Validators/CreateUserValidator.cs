using FluentValidation;
using SmartInventorySystem.Application.DTOs.Users;
using SmartInventorySystem.Domain.Constants;

namespace SmartInventorySystem.Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => role is Roles.Admin or Roles.Employee)
            .WithMessage($"Role must be '{Roles.Admin}' or '{Roles.Employee}'.");
    }
}
