namespace Dddify.Admin.Domain.Events.Users;

public record UserDeletedDomainEvent(Guid UserId, string Email) : IDomainEvent;
