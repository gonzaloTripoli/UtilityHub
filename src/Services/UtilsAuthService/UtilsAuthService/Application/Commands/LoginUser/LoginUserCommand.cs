using MediatR;

using UtilsAuthService.Application.DTOs;

namespace UtilsAuthService.Application.Commands.LoginUser
{
    public sealed record class LoginUserCommand(string Email, string Password) : IRequest<LoginUserDto>;
}
