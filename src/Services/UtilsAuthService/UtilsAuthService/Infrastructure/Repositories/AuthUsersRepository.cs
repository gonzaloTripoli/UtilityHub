using Microsoft.EntityFrameworkCore;
using UtilsAuthService.Application.Commands.LoginUser;
using UtilsAuthService.Domain.Entities;
using UtilsAuthService.Domain.Interfaces;
using UtilsAuthService.Infrastructure.Persistance;

namespace UtilsAuthService.Infrastructure.Repositories
{
    public class AuthUsersRepository : IAuthUsersRepository
    {
        private readonly AuthDbContext _db;
        public AuthUsersRepository(AuthDbContext db) => _db = db;

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
            _db.Users.AnyAsync(u => u.Email == email, ct);

        public async Task AddAsync(AuthUser user, CancellationToken ct)
        {
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> LoginAsync(LoginUserCommand user, CancellationToken ct) 
        {
        
        }
    }
}
