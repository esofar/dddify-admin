using Dddify.Admin.Domain.Aggregates.Departments;
using Dddify.Admin.Domain.Aggregates.Lookups;
using Dddify.Admin.Domain.Aggregates.Permissions;
using Dddify.Admin.Domain.Aggregates.Roles;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Lookup> Lookups => Set<Lookup>();
    public DbSet<Domain.Aggregates.Sessions.Session> Sessions => base.Set<Domain.Aggregates.Sessions.Session>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyDefaultConventions();

        base.OnModelCreating(modelBuilder);
    }
}
