using UtilsUserService.Domain.Entities;

namespace UtilsUserService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct);
        Task AddAsync(User user, CancellationToken ct);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
