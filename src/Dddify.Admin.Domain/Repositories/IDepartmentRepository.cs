using Dddify.Admin.Domain.Aggregates.Departments;

namespace Dddify.Admin.Domain.Repositories;

public interface IDepartmentRepository : IRepository<Department, Guid>
{
    Task<bool> IsNameUniqueAsync(string name, Guid? parentId, Guid? excludeId = null);
    Task<List<Department>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Department>> GetChildrenAsync(Guid parentId, bool recursive, CancellationToken cancellationToken = default);
    Task<List<Department>> GetDescendantsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Department>> GetListByPathPrefixAsync(string pathPrefix, CancellationToken cancellationToken = default);
}