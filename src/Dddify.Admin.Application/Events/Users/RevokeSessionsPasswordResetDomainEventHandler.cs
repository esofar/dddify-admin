using Dddify.Admin.Application.Commands.Sessions;
using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class RevokeSessionsPasswordResetDomainEventHandler(
    ISender sender) : IDomainEventHandler<UserPasswordResetDomainEvent>
{
    public async Task Handle(UserPasswordResetDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(
            new RevokeUserSessionsCommand(@event.UserId, "password_reset"),
            cancellationToken);
    }
}
