using Dddify.Admin.Application.Dtos.Departments;

namespace Dddify.Admin.Application.Queries.Departments;

public record SearchDepartmentsQuery(
    string? Name,
    string? Type,
    bool? IsEnabled) : IQuery<IEnumerable<DepartmentListDto>>;

public class SearchDepartmentsQueryHandler(IDepartmentRepository departmentRepository) : IQueryHandler<SearchDepartmentsQuery, IEnumerable<DepartmentListDto>>
{
    public async Task<IEnumerable<DepartmentListDto>> Handle(SearchDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var departments = await departmentRepository
            .AsQueryable()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => c.Name.Contains(query.Name!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Type), c => c.Type == query.Type)
            .WhereIf(query.IsEnabled.HasValue, c => c.IsEnabled == query.IsEnabled)
            .OrderBy(c => c.Order)
            .ToListAsync(cancellationToken);

        return departments.Adapt<IEnumerable<DepartmentListDto>>();
    }
}