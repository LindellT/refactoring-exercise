namespace Infrastructure;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes, added to dependency injection with RegisterInfrastructure
// Not sealed to allow mocking with NSubstitute
internal class UserContext(DbContextOptions options) : DbContext(options)
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
{
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