namespace Dddify.Admin.Infrastructure.Session;

public sealed class SessionCleanupHostedService(
    IServiceScopeFactory serviceScopeFactory,
    IDistributedLock distributedLock,
    IOptions<SessionCleanupOptions> options,
    ILogger<SessionCleanupHostedService> logger) : BackgroundService
{
    private const string CleanupLockResource = "session-cleanup";
    private static readonly TimeSpan CleanupLockLeaseTime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CleanupLockAcquireTimeout = TimeSpan.Zero;

    private readonly SessionCleanupOptions cleanupOptions = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(cleanupOptions.IntervalMinutes));

        if (cleanupOptions.RunOnStartup)
        {
            await CleanupAsync(stoppingToken);
        }

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanupAsync(stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var cleanupLock = await distributedLock.AcquireAsync(
                CleanupLockResource,
                CleanupLockLeaseTime,
                CleanupLockAcquireTimeout,
                cancellationToken: cancellationToken);

            using var scope = serviceScopeFactory.CreateScope();
            var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            var now = clock.UtcNow;

            await sessionRepository.DeleteExpiredOrRevokedSessionsAsync(
                now,
                now.AddDays(-cleanupOptions.RevokedRetentionDays),
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (TimeoutException)
        {
            logger.LogDebug("Skipped session cleanup because another instance holds the cleanup lock.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to cleanup expired or revoked sessions.");
        }
    }
}
