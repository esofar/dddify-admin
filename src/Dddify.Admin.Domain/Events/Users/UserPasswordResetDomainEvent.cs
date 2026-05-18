namespace Dddify.Admin.Domain.Events.Users;

public record UserPasswordResetDomainEvent(Guid UserId, string Email, string NewPassword) : IDomainEvent;