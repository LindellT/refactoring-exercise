using Domain;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure;

internal class UserContext : DbContext
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
        base.OnModelCreating(modelBuilder);
    }
}