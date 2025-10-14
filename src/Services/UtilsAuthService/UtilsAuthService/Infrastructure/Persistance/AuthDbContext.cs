using Microsoft.EntityFrameworkCore;
using UtilsAuthService.Domain.Entities;

namespace UtilsAuthService.Infrastructure.Persistance
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> opt) : base(opt) { }
        public DbSet<AuthUser> Users => Set<AuthUser>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<AuthUser>(e =>
            {
                e.ToTable("auth_users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Email).IsRequired().HasMaxLength(320);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.PasswordHash).IsRequired();
            });
        }
    }
}
