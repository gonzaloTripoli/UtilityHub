using FluentValidation;
namespace UtilsAuthService.Application.Commands.LoginUser
{
    public class LoginUserValidator : AbstractValidator<LoginUserCommand>
    {

        public LoginUserValidator() 
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

        }


    }
}
