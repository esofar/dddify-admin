namespace Dddify.Admin.Domain.Events.Users;

public record UserCreatedDomainEvent(
    Guid UserId,
    string FullName,
    string Email,
    string PhoneNumber) : IDomainEvent;