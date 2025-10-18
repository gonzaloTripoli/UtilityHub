using Microsoft.EntityFrameworkCore;
using UtilsUserService.Domain.Entities;
using UtilsUserService.Domain.Interfaces;
using UtilsUserService.Infrastructure.Persistence;

namespace UtilsUserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UsersDbContext _db;
        public UserRepository(UsersDbContext db) => _db = db;

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
            _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
       _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);


        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct) =>
            _db.Users.AsNoTracking().AnyAsync(u => u.Id == id, ct);

        public Task AddAsync(User user, CancellationToken ct) =>
            _db.Users.AddAsync(user, ct).AsTask();

        public Task SaveChangesAsync(CancellationToken ct) =>
            _db.SaveChangesAsync(ct);
    }
}
