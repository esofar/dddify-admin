using Dddify.Admin.Infrastructure.Caching;
using StackExchange.Redis;

namespace Dddify.Admin.Infrastructure.Locking;

public sealed class RedisDistributedLock(
    IConnectionMultiplexer connectionMultiplexer,
    IOptions<RedisOptions> options,
    ILogger<RedisDistributedLock> logger) : IDistributedLock
{
    private const int DefaultRetryDelayMilliseconds = 100;

    public async Task<IDistributedLockHandle> AcquireAsync(
        string resource,
        TimeSpan leaseTime,
        TimeSpan? timeout = null,
        bool autoRenew = true,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Lock resource cannot be empty.", nameof(resource));
        }

        if (leaseTime <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(leaseTime), "Lock lease time must be positive.");
        }

        if (timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Lock timeout cannot be negative.");
        }

        var database = connectionMultiplexer.GetDatabase();
        var key = BuildKey(resource);
        var token = Guid.NewGuid().ToString("N");
        var retryDelay = GetRetryDelay(leaseTime);
        var startedAt = DateTimeOffset.UtcNow;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await TrySetLockAsync(database, key, token, leaseTime))
            {
                return new RedisDistributedLockHandle(
                    database,
                    key,
                    resource,
                    token,
                    leaseTime,
                    autoRenew,
                    logger);
            }

            if (timeout == TimeSpan.Zero || (timeout.HasValue && DateTimeOffset.UtcNow - startedAt >= timeout.Value))
            {
                throw new TimeoutException($"Failed to acquire distributed lock '{resource}' within {timeout ?? TimeSpan.Zero}.");
            }

            var delay = retryDelay;

            if (timeout.HasValue)
            {
                var remaining = timeout.Value - (DateTimeOffset.UtcNow - startedAt);
                delay = remaining < retryDelay ? remaining : retryDelay;
            }

            if (delay <= TimeSpan.Zero)
            {
                throw new TimeoutException($"Failed to acquire distributed lock '{resource}' within {timeout ?? TimeSpan.Zero}.");
            }

            await Task.Delay(delay, cancellationToken);
        }
    }

    internal static long GetRedisExpiryMilliseconds(TimeSpan leaseTime)
    {
        return Math.Max(1, (long)Math.Ceiling(leaseTime.TotalMilliseconds));
    }

    private string BuildKey(string resource)
    {
        var prefix = options.Value.InstanceName ?? string.Empty;
        return $"{prefix}lock:{resource}";
    }

    private static async Task<bool> TrySetLockAsync(IDatabase database, RedisKey key, RedisValue token, TimeSpan leaseTime)
    {
        var milliseconds = GetRedisExpiryMilliseconds(leaseTime);
        var result = await database.ExecuteAsync("SET", key, token, "NX", "PX", milliseconds);

        return string.Equals((string?)result, "OK", StringComparison.OrdinalIgnoreCase);
    }

    private static TimeSpan GetRetryDelay(TimeSpan leaseTime)
    {
        var retryDelay = TimeSpan.FromMilliseconds(DefaultRetryDelayMilliseconds);
        var maxDelay = TimeSpan.FromMilliseconds(Math.Max(10, leaseTime.TotalMilliseconds / 10));

        return retryDelay < maxDelay ? retryDelay : maxDelay;
    }
}
