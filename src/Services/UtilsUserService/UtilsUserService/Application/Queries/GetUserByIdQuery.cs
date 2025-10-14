using MediatR;
using UtilsUserService.Application.DTOs;

namespace UtilsUserService.Application.Queries;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
