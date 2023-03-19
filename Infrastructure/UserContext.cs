using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

internal sealed class UserContext : DbContext
{
    public UserContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<UserEntity> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<HashedPassword>();
        modelBuilder.Ignore<ValidEmailAddress>();
        modelBuilder.Entity<UserEntity>().OwnsOne(x => x.HashedPassword);
        modelBuilder.Entity<UserEntity>().OwnsOne(x => x.Email);
        modelBuilder.Entity<UserEntity>().HasQueryFilter(x => !x.IsDeleted);
        
        base.OnModelCreating(modelBuilder);
    }
}