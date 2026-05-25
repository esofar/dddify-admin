using Dddify.Admin.Domain.Aggregates.Sessions;

namespace Dddify.Admin.Application.Commands.Sessions;

public record RevokeUserSessionsCommand(
    Guid UserId,
    string Reason) : ICommand;

public class RevokeUserSessionsCommandValidator : AbstractValidator<RevokeUserSessionsCommand>
{
    public RevokeUserSessionsCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();

        RuleFor(c => c.Reason)
            .NotEmpty()
            .MaximumLength(Session.MaxRevokedReasonLength);
    }
}

public class RevokeUserSessionsCommandHandler(
    ISessionRepository sessionRepository,
    IClock clock) : ICommandHandler<RevokeUserSessionsCommand>
{
    public async Task Handle(RevokeUserSessionsCommand command, CancellationToken cancellationToken)
    {
        await sessionRepository.RevokeUserSessionsAsync(
            command.UserId,
            command.Reason,
            clock.UtcNow,
            cancellationToken);
    }
}
