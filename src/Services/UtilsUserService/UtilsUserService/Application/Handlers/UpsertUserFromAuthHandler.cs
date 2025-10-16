using MediatR;
using UtilsUserService.Application.Commands.UpsertUserFromAuth;
using UtilsUserService.Domain.Entities;
using UtilsUserService.Domain.Interfaces;

namespace UtilsUserService.Application.Handlers;

public sealed class UpsertUserFromAuthHandler
    : IRequestHandler<UpsertUserFromAuthCommand, Unit>
{
    private readonly IUserRepository _repo;
    public UpsertUserFromAuthHandler(IUserRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpsertUserFromAuthCommand req, CancellationToken ct)
    {
        var exists = await _repo.ExistsByIdAsync(req.UserId, ct);
        if (!exists)
        {
            await _repo.AddAsync(new User
            {
                Id = req.UserId,
                Email = req.Email,
                CreatedAt = req.CreatedAtUtc
            }, ct);
            await _repo.SaveChangesAsync(ct);
        }
        return Unit.Value;
    }
}
