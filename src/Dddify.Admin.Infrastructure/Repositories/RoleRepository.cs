using Dddify.Admin.Domain.Aggregates.Roles;

namespace Dddify.Admin.Infrastructure.Repositories;

public class RoleRepository(ApplicationDbContext context) : RepositoryBase<ApplicationDbContext, Role, Guid>(context), IRoleRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Role?> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeRoleId = null)
    {
        return !await _context.Roles
            .WhereIf(excludeRoleId != null, c => c.Id != excludeRoleId)
            .AnyAsync(user => user.Name == name);
    }
}
