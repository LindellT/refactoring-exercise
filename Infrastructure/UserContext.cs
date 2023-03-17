using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

internal class UserContext : DbContext
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
}