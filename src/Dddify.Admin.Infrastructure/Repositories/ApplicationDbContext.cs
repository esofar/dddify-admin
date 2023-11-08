namespace Dddify.Admin.Infrastructure.Repositories;

public class ApplicationDbContext : AppDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options, InternalInterceptor internalInterceptor)
        : base(options, internalInterceptor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Dictionary> Dictionaries => Set<Dictionary>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();
    public DbSet<Permission> Permissions => Set<Permission>();
}