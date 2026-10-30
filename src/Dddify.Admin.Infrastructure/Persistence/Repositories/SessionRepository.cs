using Dddify.Admin.Domain.Aggregates.Sessions;

namespace Dddify.Admin.Infrastructure.Persistence.Repositories;

public class SessionRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext,  Session, Guid>(context), ISessionRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Session?> GetByRefreshTokenHashAsync(string refreshTokenHash, string deviceId, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(
                s => (s.RefreshTokenHash == refreshTokenHash || s.PreviousRefreshTokenHash == refreshTokenHash) &&
                    s.DeviceId == deviceId,
                cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetActiveSessionsAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.ExpiresAt > now)
            .ToListAsync(cancellationToken);
    }

    public async Task<Session?> GetActiveSessionAsync(Guid userId, string deviceId, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(
                s => s.UserId == userId &&
                    s.DeviceId == deviceId &&
                    s.RevokedAt == null &&
                    s.ExpiresAt > now,
                cancellationToken);
    }

    public async Task RevokeUserSessionsAsync(Guid userId, string reason, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        var sessions = await GetActiveSessionsAsync(userId, now, cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke(reason, now);
        }
    }

    public async Task RevokeUserDeviceSessionAsync(Guid userId, string deviceId, string reason, DateTimeOffset now, CancellationToken cancellationToken = default)
    {
        var session = await GetActiveSessionAsync(userId, deviceId, now, cancellationToken);

        session?.Revoke(reason, now);
    }

    public async Task DeleteExpiredOrRevokedSessionsAsync(DateTimeOffset expiresBefore, DateTimeOffset revokedBefore, CancellationToken cancellationToken = default)
    {
        await _context.Sessions
            .Where(s => s.ExpiresAt < expiresBefore || (s.RevokedAt != null && s.RevokedAt < revokedBefore))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
