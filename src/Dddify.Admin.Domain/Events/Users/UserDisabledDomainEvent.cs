namespace Dddify.Admin.Domain.Events.Users;

public record UserDisabledDomainEvent(Guid UserId, string Email) : IDomainEvent;
