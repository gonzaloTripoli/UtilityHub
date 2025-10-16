using MediatR;

namespace UtilsUserService.Application.Commands.UpsertUserFromAuth;

public sealed record UpsertUserFromAuthCommand(
    Guid UserId, string Email, DateTime CreatedAtUtc
) : IRequest<Unit>;