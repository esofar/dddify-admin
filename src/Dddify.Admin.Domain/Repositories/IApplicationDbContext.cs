namespace Dddify.Admin.Domain.Repositories;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    void ResetConcurrencyStamp<TEntity>(TEntity entity, string concurrencyStamp)
        where TEntity : IEntity;

    DbSet<User> Users { get;  }

    DbSet<Role> Roles { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<Dictionary> Dictionaries { get; }

    DbSet<DictionaryItem> DictionaryItems { get; }

    DbSet<Permission> Permissions { get; }
}