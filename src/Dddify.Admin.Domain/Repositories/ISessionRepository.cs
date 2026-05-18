using Dddify.Admin.Domain.Aggregates.Sessions;

namespace Dddify.Admin.Domain.Repositories;

public interface ISessionRepository : IRepository<Session, Guid>
{
    Task<Session?> GetByRefreshTokenHashAsync(string refreshTokenHash, string deviceId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Session>> GetActiveSessionsAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task<Session?> GetActiveSessionAsync(Guid userId, string deviceId, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task RevokeUserSessionsAsync(Guid userId, string reason, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task RevokeUserDeviceSessionAsync(Guid userId, string deviceId, string reason, DateTimeOffset now, CancellationToken cancellationToken = default);

    Task DeleteExpiredOrRevokedSessionsAsync(DateTimeOffset expiresBefore, DateTimeOffset revokedBefore, CancellationToken cancellationToken = default);
}
