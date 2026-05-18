using Dddify.Admin.Application.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record SwitchUserRoleCommand(Guid UserId, Guid RoleId) : ICommand;

public class SwitchUserRoleCommandValidator : AbstractValidator<SwitchUserRoleCommand>
{
    public SwitchUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}

public class SwitchUserRoleCommandHandler(IUserRepository userRepository, IDistributedCache distributedCache) : ICommandHandler<SwitchUserRoleCommand>
{
    public async Task Handle(SwitchUserRoleCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserWithRolesAsync(command.UserId, cancellationToken)
            ?? throw new UserNotFoundException(command.UserId);

        user.SwitchCurrentRole(command.RoleId);

        await distributedCache.RemoveAsync(CacheKeys.User.Roles(user.Id), cancellationToken);
    }
}
