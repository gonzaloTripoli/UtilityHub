using UtilsAuthService.Domain.Entities;

namespace UtilsAuthService.Domain.Interfaces
{
    public interface IAuthUsersRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task AddAsync(AuthUser user, CancellationToken ct);
        Task<AuthUser?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
