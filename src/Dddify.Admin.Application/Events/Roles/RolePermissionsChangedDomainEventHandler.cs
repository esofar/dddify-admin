using Dddify.Admin.Domain.Events.Roles;
using Microsoft.Extensions.Logging;

namespace Dddify.Admin.Application.Events.Roles;

public class RolePermissionsChangedDomainEventHandler(ILogger<RolePermissionsChangedDomainEventHandler> logger) : IDomainEventHandler<RolePermissionsChangedDomainEvent>
{
    public Task Handle(RolePermissionsChangedDomainEvent @event, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Handling RolePermissionsChangedDomainEvent for RoleId: {RoleId}", @event.RoleId);
        }

        return Task.CompletedTask;
    }
}
