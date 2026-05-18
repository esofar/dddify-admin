namespace Dddify.Admin.Application.Commands.Auths;

public record LogoutCommand(Guid UserId, string? DeviceId = null) : ICommand;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();
    }
}

public class LogoutCommandHandler(ISessionRepository sessionRepository, IClock clock) : ICommandHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var now = clock.UtcNow;

        if (command.DeviceId is not null)
        {
            await sessionRepository.RevokeUserDeviceSessionAsync(command.UserId, command.DeviceId, "logout", now, cancellationToken);
        }
        else
        {
            await sessionRepository.RevokeUserSessionsAsync(command.UserId, "logout_all", now, cancellationToken);
        }
    }
}
