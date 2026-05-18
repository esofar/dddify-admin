using Dddify.Admin.Domain.Aggregates.Departments;

namespace Dddify.Admin.Infrastructure.Repositories;

public class DepartmentRepository(ApplicationDbContext context) : RepositoryBase<ApplicationDbContext, Department, Guid>(context), IDepartmentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> IsNameUniqueAsync(string name, Guid? parentId, Guid? excludeId = null)
    {
        return !await _context.Departments
            .WhereIf(excludeId != null, c => c.Id != excludeId)
            .AnyAsync(c => c.Name == name && c.ParentId == parentId);
    }

    public async Task<List<Department>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Where(d => ids.Contains(d.Id))
            .OrderBy(d => d.Level)
            .ThenBy(d => d.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Department>> GetChildrenAsync(Guid parentId, bool recursive, CancellationToken cancellationToken = default)
    {
        var parent = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == parentId, cancellationToken);

        if (parent is null) return [];

        if (!recursive)
        {
            return await _context.Departments
                .Where(d => d.ParentId == parentId)
                .OrderBy(d => d.Order)
                .ToListAsync(cancellationToken);
        }

        return await GetDescendantsAsync(parentId, cancellationToken);
    }

    public async Task<List<Department>> GetDescendantsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (department is null) return [];

        return await _context.Departments
            .Where(d => d.Path.StartsWith(department.Path) && d.Id != department.Id)
            .OrderBy(d => d.Level)
            .ThenBy(d => d.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Department>> GetListByPathPrefixAsync(string pathPrefix, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pathPrefix)) return [];

        return await _context.Departments
            .Where(d => d.Path.StartsWith(pathPrefix))
            .OrderBy(d => d.Level)
            .ThenBy(d => d.Order)
            .ToListAsync(cancellationToken);
    }
}