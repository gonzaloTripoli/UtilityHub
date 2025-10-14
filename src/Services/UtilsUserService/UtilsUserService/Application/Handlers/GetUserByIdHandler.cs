using MediatR;
using UtilsUserService.Application.DTOs;
using UtilsUserService.Application.Queries;
using UtilsUserService.Domain.Interfaces;

namespace UtilsUserService.Application.Handlers;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repo;
    public GetUserByIdHandler(IUserRepository repo) => _repo = repo;

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _repo.GetByIdAsync(request.Id, ct);
        return user is null ? null : new UserDto(user.Id, user.Email, user.Role, user.CreatedAt);
    }
}
