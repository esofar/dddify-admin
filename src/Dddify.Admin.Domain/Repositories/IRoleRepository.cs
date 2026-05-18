using Dddify.Admin.Domain.Aggregates.Roles;

namespace Dddify.Admin.Domain.Repositories;

public interface IRoleRepository : IRepository<Role, Guid>
{
    Task<Role?> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeRoleId = null);
}