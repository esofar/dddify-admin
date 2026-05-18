namespace Dddify.Admin.Domain.Events.Users;

public sealed record UserLoginSucceededDomainEvent(
    Guid UserId,
    string DeviceId,
    string DeviceName,
    string? IpAddress,
    string? UserAgent,
    string RefreshTokenHash,
    DateTimeOffset RefreshTokenExpiresAt,
    bool IsPersistent) : IDomainEvent;
