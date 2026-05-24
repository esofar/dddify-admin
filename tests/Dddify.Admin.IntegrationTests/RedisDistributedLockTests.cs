using Dddify.Admin.Application.Services.Locking;
using Dddify.Admin.Infrastructure.Caching;
using Dddify.Admin.Infrastructure.Locking;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Dddify.Admin.IntegrationTests;

public sealed class RedisDistributedLockTests : IAsyncLifetime
{
    private const string RedisConfigurationEnvironmentVariable = "REDIS_TEST_CONFIGURATION";

    private IConnectionMultiplexer? connection;
    private IDistributedLock? distributedLock;

    public async Task InitializeAsync()
    {
        var configuration = Environment.GetEnvironmentVariable(RedisConfigurationEnvironmentVariable)
            ?? "localhost:6379,password=app_pwd,abortConnect=false";

        try
        {
            connection = await ConnectionMultiplexer.ConnectAsync(configuration);

            if (!connection.IsConnected)
            {
                return;
            }

            distributedLock = new RedisDistributedLock(
                connection,
                Options.Create(new RedisOptions
                {
                    Configuration = configuration,
                    InstanceName = "it:",
                }),
                NullLogger<RedisDistributedLock>.Instance);
        }
        catch (RedisConnectionException)
        {
            connection = null;
        }
    }

    public async Task DisposeAsync()
    {
        if (connection is not null)
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }
    }

    [Fact]
    public async Task AcquireAsync_Allows_Only_One_Holder_At_A_Time_Under_High_Concurrency()
    {
        if (distributedLock is null)
        {
            return;
        }

        var activeHolders = 0;
        var maxActiveHolders = 0;
        var successfulEntries = 0;
        var resource = $"high-concurrency:{Guid.NewGuid():N}";

        var tasks = Enumerable.Range(0, 64).Select(async _ =>
        {
            await using var handle = await distributedLock.AcquireAsync(
                resource,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(10),
                cancellationToken: CancellationToken.None);

            var active = Interlocked.Increment(ref activeHolders);
            maxActiveHolders = Math.Max(maxActiveHolders, active);

            await Task.Delay(25, CancellationToken.None);

            Interlocked.Decrement(ref activeHolders);
            Interlocked.Increment(ref successfulEntries);
        });

        await Task.WhenAll(tasks);

        Assert.Equal(64, successfulEntries);
        Assert.Equal(1, maxActiveHolders);
    }

    [Fact]
    public async Task AcquireAsync_Times_Out_When_Lock_Is_Held()
    {
        if (distributedLock is null)
        {
            return;
        }

        var resource = $"timeout:{Guid.NewGuid():N}";

        await using var handle = await distributedLock.AcquireAsync(
            resource,
            TimeSpan.FromSeconds(2),
            TimeSpan.Zero,
            autoRenew: false,
            cancellationToken: CancellationToken.None);

        await Assert.ThrowsAsync<TimeoutException>(() =>
            distributedLock.AcquireAsync(
                resource,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromMilliseconds(200),
                autoRenew: false,
                cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task Watchdog_Renews_Lock_Beyond_Original_Lease()
    {
        if (distributedLock is null)
        {
            return;
        }

        var resource = $"watchdog:{Guid.NewGuid():N}";

        await using var handle = await distributedLock.AcquireAsync(
            resource,
            TimeSpan.FromMilliseconds(500),
            TimeSpan.Zero,
            autoRenew: true,
            cancellationToken: CancellationToken.None);

        await Task.Delay(1400, CancellationToken.None);

        await Assert.ThrowsAsync<TimeoutException>(() =>
            distributedLock.AcquireAsync(
                resource,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(100),
                autoRenew: false,
                cancellationToken: CancellationToken.None));
    }
}
