using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record RefreshUsersRoleCommand(Guid RoleId, string RoleName) : ICommand;

public class RefreshUsersRoleCommandValidator : AbstractValidator<RefreshUsersRoleCommand>
{
    public RefreshUsersRoleCommandValidator()
    {
        RuleFor(c => c.RoleId)
            .NotEmpty();

        RuleFor(c => c.RoleName)
            .NotEmpty()
            .MaximumLength(UserRole.MaxRoleNameLength);
    }
}

public class RefreshUsersRoleCommandHandler(IUserRepository userRepository) : ICommandHandler<RefreshUsersRoleCommand>
{
    public async Task Handle(RefreshUsersRoleCommand command, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersByRoleIdAsync(command.RoleId, cancellationToken);

        foreach (var user in users)
        {
            user.ChangeRoleName(command.RoleId, command.RoleName);
        }
    }
}
