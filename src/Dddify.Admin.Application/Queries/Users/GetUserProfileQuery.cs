using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Application.Queries.Users;

public record GetUserProfileQuery() : IQuery<UserProfileDto>;

public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetUserProfileQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == _currentUser.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(_currentUser.Id);
        }

        return user.Adapt<UserProfileDto>();
    }
}