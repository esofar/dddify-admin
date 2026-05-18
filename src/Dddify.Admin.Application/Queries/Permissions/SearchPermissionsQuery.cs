using Dddify.Admin.Application.Dtos.Permissions;

namespace Dddify.Admin.Application.Queries.Permissions;

public record SearchPermissionsQuery(string? Name, string? Code) : IQuery<IEnumerable<PermissionDto>>;

public class SearchPermissionsQueryHandler(IPermissionRepository permissionRepository) : IQueryHandler<SearchPermissionsQuery, IEnumerable<PermissionDto>>
{
    public async Task<IEnumerable<PermissionDto>> Handle(SearchPermissionsQuery query, CancellationToken cancellationToken)
    {
        var permissions = await permissionRepository
            .AsQueryable()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => EF.Functions.Like(c.Name, $"%{query.Name}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Code), c => EF.Functions.Like(c.Code, $"%{query.Code}%"))
            .OrderBy(c => c.Order)
            .ToListAsync(cancellationToken);

        return permissions.Select(c => new PermissionDto(c.Id, c.ParentId, c.Code, c.Name, c.Type.ToString(), c.Order));
    }
}
