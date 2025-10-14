using MediatR;
using UtilsUserService.Application.DTOs;

namespace UtilsUserService.Application.Commands
{
    public record CreateUserCommand(string Email, string Password, string? Role)
      : IRequest<UserDto>;
}
