using FluentValidation;

namespace UtilsAuthService.Application.Commands.DeleteUser
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {

        public DeleteUserValidator() {

            RuleFor(x => x.id).NotEmpty();
            

        }


    }
}
