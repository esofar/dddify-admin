using Dddify.Admin.Domain.Aggregates.Sessions;

namespace Dddify.Admin.Application.Commands.Sessions;

public sealed record CreateSessionCommand(
    Guid UserId,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    string RefreshTokenHash,
    DateTimeOffset ExpiresAt,
    bool IsPersistent) : ICommand;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();

        RuleFor(c => c.DeviceId)
            .NotEmpty()
            .MaximumLength(Session.MaxDeviceIdLength);

        RuleFor(c => c.DeviceName)
            .NotEmpty()
            .MaximumLength(Session.MaxDeviceNameLength);

        RuleFor(c => c.IpAddress)
            .MaximumLength(Session.MaxIpAddressLength);

        RuleFor(c => c.UserAgent)
            .MaximumLength(Session.MaxUserAgentLength);

        RuleFor(c => c.RefreshTokenHash)
            .NotEmpty()
            .MaximumLength(Session.MaxRefreshTokenHashLength);

        RuleFor(c => c.ExpiresAt)
            .NotEmpty();
    }
}

public class CreateSessionCommandHandler(
    ISessionRepository sessionRepository,
    IGuidGenerator guidGenerator,
    IClock clock) : ICommandHandler<CreateSessionCommand>
{
    public async Task Handle(CreateSessionCommand command, CancellationToken cancellationToken)
    {
        var now = clock.UtcNow;

        var activeSession = await sessionRepository.GetActiveSessionAsync(
            command.UserId,
            command.DeviceId,
            now,
            cancellationToken);

        activeSession?.Revoke("replaced_by_new_login", now);

        var session = new Session(
            guidGenerator.Create(),
            command.UserId,
            command.DeviceId,
            command.DeviceName,
            command.IpAddress,
            command.UserAgent,
            command.RefreshTokenHash,
            command.ExpiresAt,
            now,
            command.IsPersistent);

        await sessionRepository.AddAsync(session, cancellationToken);
    }
}
