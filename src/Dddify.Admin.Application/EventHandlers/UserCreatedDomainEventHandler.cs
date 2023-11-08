using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.EventHandlers;

public class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
    public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}