using FluentValidation;

namespace UtilsUserService.Application.Commands;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Role).Must(r => r is null or "user" or "admin")
            .WithMessage("Role inválido (user/admin).");
    }
}
