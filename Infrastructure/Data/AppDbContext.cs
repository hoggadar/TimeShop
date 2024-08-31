using Microsoft.EntityFrameworkCore;
using TimeShop.Domain.Entities;

namespace TimeShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
