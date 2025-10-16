using Microsoft.EntityFrameworkCore;
using UtilsUserService.Domain.Entities;

namespace UtilsUserService.Infrastructure.Persistence
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Email).IsRequired().HasMaxLength(320);
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            });
        }
    }
}
