using Dddify.Admin.Application.Caching;
using Dddify.Admin.Infrastructure.Caching;
using Dddify.EntityFrameworkCore;
using Dddify.Messaging.Behaviors;
using Dddify.Messaging.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data.Common;

namespace Dddify.Admin.IntegrationTests;

public class CacheInvalidationTests
{
    [Fact]
    public async Task Cache_Is_Removed_Only_After_The_Database_Commit()
    {
        await using var store = await TestStore.CreateAsync();
        await store.Cache.SetStringAsync("roles", "old roles");
        store.Cache.OnRemove = _ =>
        {
            using var observer = store.OpenObserver();
            Assert.Equal("updated", observer.Rows.Single().Name);
        };

        await store.RunCommand(async token =>
        {
            var row = await store.Context.Rows.SingleAsync(token);
            row.Name = "updated";
            store.Queue.Enqueue("roles");
            await store.Context.SaveChangesAsync(token);

            Assert.Equal("old roles", await store.Cache.GetStringAsync("roles", token));
            Assert.Empty(store.Cache.Removed);
            using var observer = store.OpenObserver();
            Assert.Equal("original", observer.Rows.Single().Name);
            return Unit.Value;
        });

        Assert.Null(await store.Cache.GetStringAsync("roles"));
        Assert.Equal("roles", Assert.Single(store.Cache.Removed));
    }

    [Fact]
    public async Task Nested_Command_Waits_For_The_Outer_Commit_And_Deduplicates_Keys()
    {
        await using var store = await TestStore.CreateAsync();
        await store.Cache.SetStringAsync("roles", "old roles");

        await store.RunCommand(async token =>
        {
            store.Queue.Enqueue("roles");
            await store.RunCommand(async nestedToken =>
            {
                var row = await store.Context.Rows.SingleAsync(nestedToken);
                row.Name = "nested update";
                store.Queue.Enqueue("roles");
                return Unit.Value;
            });

            Assert.Equal("old roles", await store.Cache.GetStringAsync("roles", token));
            Assert.Empty(store.Cache.Removed);
            return Unit.Value;
        });

        Assert.Equal("roles", Assert.Single(store.Cache.Removed));
    }

    [Fact]
    public async Task Rollback_Discards_Invalidations_And_Does_Not_Leak_Into_The_Next_Command()
    {
        await using var store = await TestStore.CreateAsync();
        await store.Cache.SetStringAsync("roles", "old roles");

        await Assert.ThrowsAsync<InvalidOperationException>(() => store.RunCommand(async token =>
        {
            var row = await store.Context.Rows.SingleAsync(token);
            row.Name = "rolled back";
            store.Queue.Enqueue("roles");
            await store.Context.SaveChangesAsync(token);
            throw new InvalidOperationException("Command failed.");
        }));

        store.Context.ChangeTracker.Clear();
        await store.RunCommand(_ => Task.FromResult(Unit.Value));

        Assert.Equal("original", (await store.Context.Rows.SingleAsync()).Name);
        Assert.Equal("old roles", await store.Cache.GetStringAsync("roles"));
        Assert.Empty(store.Cache.Removed);
        Assert.Empty(store.Queue.Drain());
    }

    [Fact]
    public async Task Commit_Failure_Does_Not_Invalidate_Cache()
    {
        var failure = new FailCommitInterceptor();
        await using var store = await TestStore.CreateAsync(failure);
        await store.Cache.SetStringAsync("roles", "old roles");
        failure.Enabled = true;

        await Assert.ThrowsAsync<InvalidOperationException>(() => store.RunCommand(async token =>
        {
            var row = await store.Context.Rows.SingleAsync(token);
            row.Name = "not committed";
            store.Queue.Enqueue("roles");
            return Unit.Value;
        }));

        using var observer = store.OpenObserver();
        Assert.Equal("original", observer.Rows.Single().Name);
        Assert.Equal("old roles", await store.Cache.GetStringAsync("roles"));
        Assert.Empty(store.Cache.Removed);
        Assert.Empty(store.Queue.Drain());
    }

    [Fact]
    public async Task Cache_Failure_Does_Not_Fail_A_Committed_Command_Or_Skip_Other_Keys()
    {
        await using var store = await TestStore.CreateAsync();
        await store.Cache.SetStringAsync("permissions", "old permissions");
        store.Cache.OnRemove = key =>
        {
            if (key == "roles") throw new InvalidOperationException("Cache unavailable.");
        };

        await store.RunCommand(async token =>
        {
            var row = await store.Context.Rows.SingleAsync(token);
            row.Name = "committed";
            store.Queue.Enqueue("roles");
            store.Queue.Enqueue("permissions");
            return Unit.Value;
        });

        using var observer = store.OpenObserver();
        Assert.Equal("committed", observer.Rows.Single().Name);
        Assert.Null(await store.Cache.GetStringAsync("permissions"));
        Assert.Empty(store.Queue.Drain());
    }

    private sealed record TestCommand : ICommand;

    private sealed class TestStore : IAsyncDisposable
    {
        private readonly string path = Path.Combine(Path.GetTempPath(), $"dddify-cache-{Guid.NewGuid():N}.db");

        public CacheInvalidationQueue Queue { get; } = new();
        public TestCache Cache { get; } = new();
        public CacheDbContext Context { get; private set; } = null!;
        private UnitOfWork<CacheDbContext> UnitOfWork { get; set; } = null!;
        private string ConnectionString => $"Data Source={path};Pooling=False";

        public static async Task<TestStore> CreateAsync(IInterceptor? extraInterceptor = null)
        {
            var store = new TestStore();
            var options = new DbContextOptionsBuilder<CacheDbContext>()
                .UseSqlite(store.ConnectionString)
                .AddInterceptors(new CacheInvalidationInterceptor(
                    store.Queue, store.Cache, NullLogger<CacheInvalidationInterceptor>.Instance));
            if (extraInterceptor is not null) options.AddInterceptors(extraInterceptor);

            store.Context = new CacheDbContext(options.Options);
            store.UnitOfWork = new UnitOfWork<CacheDbContext>(store.Context);
            await store.Context.Database.EnsureCreatedAsync();
            store.Context.Rows.Add(new CacheRow { Id = 1, Name = "original" });
            await store.Context.SaveChangesAsync();
            return store;
        }

        public Task<Unit> RunCommand(RequestHandlerDelegate<Unit> next) =>
            new UnitOfWorkBehavior<TestCommand, Unit>(
                UnitOfWork, NullLogger<UnitOfWorkBehavior<TestCommand, Unit>>.Instance)
                .Handle(new TestCommand(), next, CancellationToken.None);

        public CacheDbContext OpenObserver() => new(new DbContextOptionsBuilder<CacheDbContext>()
            .UseSqlite(ConnectionString).Options);

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            File.Delete(path);
        }
    }

    public sealed class CacheDbContext(DbContextOptions<CacheDbContext> options) : DbContext(options)
    {
        public DbSet<CacheRow> Rows => Set<CacheRow>();
    }

    public sealed class CacheRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    private sealed class FailCommitInterceptor : DbTransactionInterceptor
    {
        public bool Enabled { get; set; }

        public override ValueTask<InterceptionResult> TransactionCommittingAsync(
            DbTransaction transaction, TransactionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = default) =>
            Enabled ? throw new InvalidOperationException("Commit failed.") : ValueTask.FromResult(result);
    }

    private sealed class TestCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> entries = new();
        public List<string> Removed { get; } = [];
        public Action<string>? OnRemove { get; set; }

        public byte[]? Get(string key) => entries.GetValueOrDefault(key);
        public Task<byte[]?> GetAsync(string key, CancellationToken token = default) => Task.FromResult(Get(key));
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => entries[key] = value;
        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            OnRemove?.Invoke(key);
            Removed.Add(key);
            entries.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Assert.Equal(CancellationToken.None, token);
            Remove(key);
            return Task.CompletedTask;
        }

        public void Refresh(string key) { }
        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;
    }
}
