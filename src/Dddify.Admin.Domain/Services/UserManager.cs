using Dddify.Timing;

namespace Dddify.Admin.Domain.Services;

public class UserManager : IDomainService
{
    private readonly IClock _clock;

    public UserManager(IClock clock)
    {
        _clock = clock;
    }
}