using Dddify.Admin.Application.Dtos.Users;

namespace Dddify.Admin.Application.Queries.Users;

public record GetUsersQuery(
    int Page,
    int Size,
    string? FullName,
    UserGender? Gender,
    Guid? OrganizationId,
    UserStatus? Status) : IQuery<IPagedResult<UserDto>>;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IPagedResult<UserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IPagedResult<UserDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(c => c.Organization)
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.FullName), c => EF.Functions.Like(c.FullName, $"%{query.FullName}%"))
            .WhereIf(query.Gender.HasValue, c => c.Gender == query.Gender)
            .WhereIf(query.OrganizationId.HasValue, c => c.OrganizationId == query.OrganizationId)
            .WhereIf(query.Status.HasValue, c => c.Status == query.Status)
            .OrderBy(c => c.CreatedAt)
            .ProjectToType<UserDto>()
            .ToPagedResultAsync(query.Page, query.Size, cancellationToken);
    }
}