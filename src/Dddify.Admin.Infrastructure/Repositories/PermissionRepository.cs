using Dddify.Admin.Domain.Aggregates.Permissions;

namespace Dddify.Admin.Infrastructure.Repositories;

public class PermissionRepository(ApplicationDbContext context) : RepositoryBase<ApplicationDbContext, Permission, Guid>(context), IPermissionRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Permission?> GetByCodeAsync(string code)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        return !await _context.Permissions
            .WhereIf(excludeId != null, c => c.Id != excludeId)
            .AnyAsync(c => c.Code == code);
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? parentId, Guid? excludeId = null)
    {
        return !await _context.Permissions
            .WhereIf(excludeId != null, c => c.Id != excludeId)
            .AnyAsync(c => c.Name == name && c.ParentId == parentId);
    }

    public async Task<bool> IsDescendantOfAsync(Guid ancestorId, Guid descendantId)
    {
        if (ancestorId == descendantId)
        {
            return true;
        }

        var allPermissions = await GetAllAsync();
        var permissionDict = allPermissions.ToDictionary(p => p.Id, p => p.ParentId);

        var currentId = descendantId;
        var visitedIds = new HashSet<Guid> { currentId };

        while (permissionDict.TryGetValue(currentId, out var parentId) && parentId.HasValue)
        {
            if (parentId.Value == ancestorId)
            {
                return true;
            }

            if (visitedIds.Contains(parentId.Value))
            {
                break;
            }

            visitedIds.Add(parentId.Value);
            currentId = parentId.Value;
        }

        return false;
    }
}
