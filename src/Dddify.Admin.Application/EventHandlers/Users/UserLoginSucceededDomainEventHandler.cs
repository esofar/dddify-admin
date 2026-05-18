using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.EventHandlers.Users;

public class UserLoginSucceededDomainEventHandler(ISender sender) : IDomainEventHandler<UserLoginSucceededDomainEvent>
{
    public Task Handle(UserLoginSucceededDomainEvent @event, CancellationToken cancellationToken)
    {
        return sender.Send(
            new CreateSessionCommand(
                @event.UserId,
                @event.DeviceId,
                @event.DeviceName,
                @event.IpAddress,
                @event.UserAgent,
                @event.RefreshTokenHash,
                @event.RefreshTokenExpiresAt,
                @event.IsPersistent),
            cancellationToken);
    }
}
