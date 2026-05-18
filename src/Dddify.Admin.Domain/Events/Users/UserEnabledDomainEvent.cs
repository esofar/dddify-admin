namespace Dddify.Admin.Domain.Events.Users;

public record UserEnabledDomainEvent(Guid UserId, string Email) : IDomainEvent;