using StackExchange.Redis;

namespace Dddify.Admin.Infrastructure.Locking;

public sealed class RedisDistributedLockHandle : IDistributedLockHandle
{
    public const string UnlockScript = """
        if redis.call('GET', KEYS[1]) == ARGV[1] then
            return redis.call('DEL', KEYS[1])
        end

        return 0
        """;

    private const string RenewScript = """
        if redis.call('GET', KEYS[1]) == ARGV[1] then
            return redis.call('PEXPIRE', KEYS[1], ARGV[2])
        end

        return 0
        """;

    private readonly IDatabase database;
    private readonly RedisKey key;
    private readonly TimeSpan leaseTime;
    private readonly ILogger logger;
    private readonly CancellationTokenSource renewCancellationTokenSource = new();
    private readonly Task? renewTask;
    private int released;

    internal RedisDistributedLockHandle(
        IDatabase database,
        RedisKey key,
        string resource,
        string token,
        TimeSpan leaseTime,
        bool autoRenew,
        ILogger logger)
    {
        this.database = database;
        this.key = key;
        this.leaseTime = leaseTime;
        this.logger = logger;

        Resource = resource;
        Token = token;
        IsAcquired = true;

        if (autoRenew)
        {
            renewTask = RenewUntilReleasedAsync(renewCancellationTokenSource.Token);
        }
    }

    public string Resource { get; }

    public string Token { get; }

    public bool IsAcquired { get; private set; }

    public async ValueTask ReleaseAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref released, 1) == 1)
        {
            return;
        }

        IsAcquired = false;

        await StopRenewalAsync();

        cancellationToken.ThrowIfCancellationRequested();

        await database.ScriptEvaluateAsync(
            UnlockScript,
            [key],
            [Token]);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await ReleaseAsync(CancellationToken.None);
        }
        finally
        {
            renewCancellationTokenSource.Dispose();
        }
    }

    private async Task StopRenewalAsync()
    {
        await renewCancellationTokenSource.CancelAsync();

        if (renewTask is null)
        {
            return;
        }

        try
        {
            await renewTask;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task RenewUntilReleasedAsync(CancellationToken cancellationToken)
    {
        var renewInterval = GetRenewInterval(leaseTime);
        var expiryMilliseconds = RedisDistributedLock.GetRedisExpiryMilliseconds(leaseTime);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(renewInterval, cancellationToken);

            var renewed = (int)await database.ScriptEvaluateAsync(
                RenewScript,
                [key],
                [Token, expiryMilliseconds]);

            if (renewed != 1)
            {
                IsAcquired = false;
                logger.LogWarning("Distributed lock renewal failed. Resource={Resource}", Resource);
                return;
            }
        }
    }

    private static TimeSpan GetRenewInterval(TimeSpan leaseTime)
    {
        var interval = TimeSpan.FromMilliseconds(leaseTime.TotalMilliseconds / 3);
        var minimum = TimeSpan.FromMilliseconds(200);

        return interval > minimum ? interval : minimum;
    }
}
