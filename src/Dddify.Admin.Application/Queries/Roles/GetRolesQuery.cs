using Dddify.Admin.Application.Dtos.Roles;
using Dddify.EntityFrameworkCore;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetRolesQuery(
    int Page,
    int Size,
    string? Code,
    string? Name) : IQuery<IPagedResult<RoleDto>>;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, IPagedResult<RoleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IPagedResult<RoleDto>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
    {
        return await _context.Roles
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Code), c => EF.Functions.Like(c.Code, $"{query.Code}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), c => EF.Functions.Like(c.Name, $"{query.Name}%"))
            .OrderBy(c => c.CreatedAt)
            .ProjectToType<RoleDto>()
            .ToPagedResultAsync(query.Page, query.Size, cancellationToken);
    }
}