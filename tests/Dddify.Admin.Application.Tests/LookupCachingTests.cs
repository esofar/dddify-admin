using Dddify.Admin.Application.Caching;
using Dddify.Admin.Application.Commands.Lookups;
using Dddify.Admin.Application.Queries.Lookups;
using Dddify.Admin.Domain.Aggregates.Lookups;
using Dddify.Admin.Domain.Exceptions.Lookups;
using Dddify.Admin.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;

namespace Dddify.Admin.Application.Tests;

public class LookupCachingTests
{
    [Fact]
    public async Task Lookup_Reads_Use_A_Short_Absolute_Ttl_And_Reuse_Shared_Cache()
    {
        var lookup = CreateLookup();
        var reads = 0;
        var repository = Stub<ILookupRepository>((_, _) =>
        {
            reads++;
            return Task.FromResult<Lookup?>(lookup);
        });
        var cache = new TestCache();
        var firstInstance = new GetLookupActiveItemsQueryHandler(repository, cache);
        var secondInstance = new GetLookupActiveItemsQueryHandler(repository, cache);

        await firstInstance.Handle(new GetLookupActiveItemsQuery(lookup.Code), default);
        await secondInstance.Handle(new GetLookupActiveItemsQuery(lookup.Code), default);

        Assert.Equal(1, reads);
        Assert.InRange(cache.LastOptions!.AbsoluteExpirationRelativeToNow!.Value.TotalSeconds, 45, 60);
        Assert.Null(cache.LastOptions.SlidingExpiration);
        Assert.Empty(cache.Removed);
    }

    [Fact]
    public async Task Command_Invalidates_Shared_Cache_After_Changing_Lookup()
    {
        var lookup = CreateLookup();
        var repository = Repository(lookup);
        var cache = new TestCache();
        var firstInstance = new GetLookupActiveItemsQueryHandler(repository, cache);
        var secondInstance = new GetLookupActiveItemsQueryHandler(repository, cache);
        Assert.Equal("Old", Assert.Single(await firstInstance.Handle(new(lookup.Code), default)).Label);
        cache.OnRemove = _ => Assert.Equal("New", Assert.Single(lookup.Items).Label);
        using var cancellation = new CancellationTokenSource();

        await new UpdateLookupItemCommandHandler(repository, cache).Handle(
            new UpdateLookupItemCommand(lookup.Id, Assert.Single(lookup.Items).Id, "New", null), cancellation.Token);

        Assert.Equal(CacheKeys.LookupActiveItems(lookup.Code), Assert.Single(cache.Removed));
        Assert.Equal(cancellation.Token, cache.LastRemoveToken);
        Assert.Equal("New", Assert.Single(await secondInstance.Handle(new(lookup.Code), default)).Label);
    }

    [Fact]
    public async Task Rejected_Command_Does_Not_Invalidate_Cache()
    {
        var lookup = CreateLookup();
        var repository = Repository(lookup);
        var cache = new TestCache();
        var query = new GetLookupActiveItemsQueryHandler(repository, cache);
        await query.Handle(new(lookup.Code), default);

        await Assert.ThrowsAsync<LookupItemNotFoundException>(() => new UpdateLookupItemCommandHandler(repository, cache)
            .Handle(new UpdateLookupItemCommand(lookup.Id, Guid.NewGuid(), "New", null), default));

        Assert.Empty(cache.Removed);
        Assert.Equal("Old", Assert.Single(await query.Handle(new(lookup.Code), default)).Label);
    }

    private static Lookup CreateLookup()
    {
        var lookup = new Lookup(Guid.NewGuid(), "gender", "Gender", null);
        lookup.AddItem(Guid.NewGuid(), "value", "Old", null);
        return lookup;
    }

    private static ILookupRepository Repository(Lookup lookup) => Stub<ILookupRepository>((method, _) =>
    {
        Assert.Equal("GetLookupWithItemsAsync", method.Name);
        return Task.FromResult<Lookup?>(lookup);
    });

    private sealed class TestCache : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> entries = new();
        public List<string> Removed { get; } = [];
        public Action<string>? OnRemove { get; set; }
        public DistributedCacheEntryOptions? LastOptions { get; private set; }
        public CancellationToken LastRemoveToken { get; private set; }

        public byte[]? Get(string key) => entries.GetValueOrDefault(key);
        public Task<byte[]?> GetAsync(string key, CancellationToken token = default) => Task.FromResult(Get(key));
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            entries[key] = value;
            LastOptions = options;
        }

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
            token.ThrowIfCancellationRequested();
            LastRemoveToken = token;
            Remove(key);
            return Task.CompletedTask;
        }

        public void Refresh(string key) { }
        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;
    }

    private static T Stub<T>(Func<MethodInfo, object?[]?, object?> callback) where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).Callback = callback;
        return proxy;
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Callback { get; set; } = default!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Callback(method!, args);
    }
}
