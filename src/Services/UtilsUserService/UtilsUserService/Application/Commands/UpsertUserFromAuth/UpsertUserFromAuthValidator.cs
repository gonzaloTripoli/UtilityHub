using FluentValidation;

namespace UtilsUserService.Application.Commands.UpsertUserFromAuth;

public sealed class UpsertUserFromAuthValidator : AbstractValidator<UpsertUserFromAuthCommand>
{
    public UpsertUserFromAuthValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
