using MediatR;

namespace UtilsAuthService.Application.Commands.LoginUser
{
   public sealed record class LoginUserCommand (string Password, string Email):IRequest<Guid>;
}
