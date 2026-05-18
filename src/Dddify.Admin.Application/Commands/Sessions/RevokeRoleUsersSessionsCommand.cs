using Dddify.Admin.Domain.Aggregates.Sessions;

namespace Dddify.Admin.Application.Commands.Sessions;

public record RevokeRoleUsersSessionsCommand(Guid RoleId, string Reason) : ICommand;

public class RevokeRoleUsersSessionsCommandValidator : AbstractValidator<RevokeRoleUsersSessionsCommand>
{
    public RevokeRoleUsersSessionsCommandValidator()
    {
        RuleFor(c => c.RoleId)
            .NotEmpty();

        RuleFor(c => c.Reason)
            .NotEmpty()
            .MaximumLength(Session.MaxRevokedReasonLength);
    }
}

public class RevokeRoleUsersSessionsCommandHandler(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IClock clock) : ICommandHandler<RevokeRoleUsersSessionsCommand>
{
    public async Task Handle(RevokeRoleUsersSessionsCommand command, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsersByRoleIdAsync(command.RoleId, cancellationToken);
        var now = clock.UtcNow;

        foreach (var user in users)
        {
            await sessionRepository.RevokeUserSessionsAsync(
                user.Id,
                command.Reason,
                now,
                cancellationToken);
        }
    }
}
