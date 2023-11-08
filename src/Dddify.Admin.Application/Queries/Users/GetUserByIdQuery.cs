using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IQuery<UserDetailDto>;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDetailDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(query.Id);
        }

        return user.Adapt<UserDetailDto>();
    }
}