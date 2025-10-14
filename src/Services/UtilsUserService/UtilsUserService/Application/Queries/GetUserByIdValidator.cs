using FluentValidation;

namespace UtilsUserService.Application.Queries;

public sealed class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
