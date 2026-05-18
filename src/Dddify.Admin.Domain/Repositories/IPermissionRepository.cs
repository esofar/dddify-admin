using Dddify.Admin.Domain.Aggregates.Permissions;

namespace Dddify.Admin.Domain.Repositories;

public interface IPermissionRepository : IRepository<Permission, Guid>
{
    Task<Permission?> GetByCodeAsync(string code);
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
    Task<bool> IsNameUniqueAsync(string name, Guid? parentId, Guid? excludeId = null);
    Task<bool> IsDescendantOfAsync(Guid ancestorId, Guid descendantId);
}