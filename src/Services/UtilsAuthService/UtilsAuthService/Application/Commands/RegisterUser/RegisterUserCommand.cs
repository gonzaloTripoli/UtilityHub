using MediatR;

namespace UtilsAuthService.Application.Commands.RegisterUser;

public sealed record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;
