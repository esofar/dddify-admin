using Dddify.Admin.Application.Dtos.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record SearchRolesQuery(
    int Current,
    int PageSize,
    string? Name) : IQuery<IPagedResult<RoleListDto>>;

public class SearchRolesQueryHandler(IRoleRepository roleRepository) : IQueryHandler<SearchRolesQuery, IPagedResult<RoleListDto>>
{
    public async Task<IPagedResult<RoleListDto>> Handle(SearchRolesQuery query, CancellationToken cancellationToken)
    {
        return await roleRepository
            .AsQueryable()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => EF.Functions.Like(c.Name, $"%{query.Name}%"))
            .OrderBy(c => c.Order)
            .ProjectToType<RoleListDto>()
            .ToPagedResultAsync(query.Current, query.PageSize, cancellationToken);
    }
}