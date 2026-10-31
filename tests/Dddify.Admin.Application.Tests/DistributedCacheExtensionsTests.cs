using Dddify.Admin.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Dddify.Admin.Application.Tests;

public class DistributedCacheExtensionsTests
{
    [Fact]
    public async Task Existing_String_Serialized_Data_Remains_Readable()
    {
        var cache = new TestCache();
        cache.Set("key", Encoding.UTF8.GetBytes("{\"Name\":\"缓存\"}"), new());

        var result = await cache.GetOrCreateJsonAsync<Entry>("key", _ =>
            throw new InvalidOperationException("Cache hit must not invoke the factory."));

        Assert.Equal(new Entry("缓存"), result);
    }

    [Fact]
    public async Task Custom_Serialization_Options_Are_Used_For_Writes_And_Reads()
    {
        var cache = new TestCache();
        var serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        var value = new Entry("缓存");
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

        var created = await cache.GetOrCreateJsonAsync("key", _ => Task.FromResult(value), options, serializerOptions);
        var cached = await cache.GetJsonAsync<Entry>("key", serializerOptions);
        var (found, valueFromTryGet) = await cache.TryGetJsonAsync<Entry>("key", serializerOptions);

        Assert.Equal(value, created);
        Assert.Equal(value, cached);
        Assert.True(found);
        Assert.Equal(value, valueFromTryGet);
        Assert.Same(options, cache.LastOptions);
        Assert.Contains("\"name\"", Encoding.UTF8.GetString(cache.Values["key"]));
    }

    [Fact]
    public async Task Cache_Miss_Forwards_Cancellation_And_Applies_Default_Expiration()
    {
        var cache = new TestCache();
        using var cancellation = new CancellationTokenSource();

        var result = await cache.GetOrCreateJsonAsync("key", token =>
        {
            Assert.Equal(cancellation.Token, token);
            return Task.FromResult("value");
        }, cancellationToken: cancellation.Token);

        Assert.Equal("value", result);
        Assert.Equal(cancellation.Token, cache.LastGetToken);
        Assert.Equal(cancellation.Token, cache.LastSetToken);
        Assert.Equal(TimeSpan.FromMinutes(5), cache.LastOptions!.AbsoluteExpirationRelativeToNow);

        await cache.SetJsonAsync("other", "value");
        Assert.Equal(TimeSpan.FromMinutes(5), cache.LastOptions!.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public async Task Default_Values_And_Empty_Collections_Are_Cache_Hits()
    {
        var cache = new TestCache();
        await cache.SetJsonAsync("number", 0);
        await cache.SetJsonAsync("flag", false);
        await cache.SetJsonAsync("items", Array.Empty<string>());

        Assert.Equal(0, await cache.GetOrCreateJsonAsync<int>("number", _ => throw new InvalidOperationException()));
        Assert.False(await cache.GetOrCreateJsonAsync<bool>("flag", _ => throw new InvalidOperationException()));
        Assert.Empty(await cache.GetOrCreateJsonAsync<string[]>("items", _ => throw new InvalidOperationException()));
    }

    [Fact]
    public async Task Null_Factory_Results_Are_Not_Cached()
    {
        var cache = new TestCache();
        var calls = 0;
        Task<string?> LoadAsync(CancellationToken _)
        {
            calls++;
            return Task.FromResult<string?>(null);
        }

        Assert.Null(await cache.GetOrCreateJsonAsync("key", LoadAsync));
        Assert.Null(await cache.GetOrCreateJsonAsync("key", LoadAsync));
        Assert.Equal(2, calls);
        Assert.Empty(cache.Values);
    }

    [Theory]
    [InlineData("")]
    [InlineData("null")]
    public async Task Empty_Or_Null_Entries_Are_Repopulated(string json)
    {
        var cache = new TestCache();
        cache.Set("key", Encoding.UTF8.GetBytes(json), new());

        Assert.Null(await cache.GetJsonAsync<string>("key"));
        Assert.Equal("value", await cache.GetOrCreateJsonAsync("key", _ => Task.FromResult("value")));
        Assert.Equal("value", await cache.GetJsonAsync<string>("key"));
    }

    [Fact]
    public async Task Cancellation_Prevents_Cache_Access_And_Factory_Execution()
    {
        var cache = new TestCache();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cache.GetOrCreateJsonAsync<string>(
            "key", _ => throw new InvalidOperationException(), cancellationToken: cancellation.Token));

        Assert.Equal(0, cache.GetCalls);
        Assert.Empty(cache.Values);
    }

    [Fact]
    public async Task Cancellation_During_Factory_Prevents_Cache_Write()
    {
        var cache = new TestCache();
        using var cancellation = new CancellationTokenSource();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => cache.GetOrCreateJsonAsync("key", _ =>
        {
            cancellation.Cancel();
            return Task.FromResult("value");
        }, cancellationToken: cancellation.Token));

        Assert.Empty(cache.Values);
    }

    [Fact]
    public async Task Invalid_Json_Is_Reported_Without_Invoking_Factory()
    {
        var cache = new TestCache();
        cache.Set("key", Encoding.UTF8.GetBytes("invalid json"), new());

        await Assert.ThrowsAsync<JsonException>(() => cache.GetOrCreateJsonAsync<string>(
            "key", _ => throw new InvalidOperationException("Factory must not hide invalid JSON.")));
    }

    [Fact]
    public async Task Factory_Failure_Does_Not_Write_Cache()
    {
        var cache = new TestCache();
        var failure = new InvalidOperationException("Source failed.");

        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            cache.GetOrCreateJsonAsync<string>("key", _ => Task.FromException<string>(failure)));

        Assert.Same(failure, actual);
        Assert.Empty(cache.Values);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Cache_Failures_Are_Reported(bool failOnWrite)
    {
        var failure = new IOException("Cache unavailable.");
        var cache = new TestCache { GetFailure = failOnWrite ? null : failure, SetFailure = failOnWrite ? failure : null };
        var calls = 0;

        var actual = await Assert.ThrowsAsync<IOException>(() => cache.GetOrCreateJsonAsync("key", _ =>
        {
            calls++;
            return Task.FromResult("value");
        }));

        Assert.Same(failure, actual);
        Assert.Equal(failOnWrite ? 1 : 0, calls);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Invalid_Keys_Are_Rejected_Before_Cache_Access(string key)
    {
        var cache = new TestCache();

        await Assert.ThrowsAsync<ArgumentException>(() => cache.SetJsonAsync(key, "value"));
        await Assert.ThrowsAsync<ArgumentException>(() => cache.GetJsonAsync<string>(key));
        await Assert.ThrowsAsync<ArgumentException>(() => cache.GetOrCreateJsonAsync(key, _ => Task.FromResult("value")));

        Assert.Equal(0, cache.GetCalls);
        Assert.Empty(cache.Values);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    public async Task TryGet_Distinguishes_Cached_Numbers_From_Missing_Values(int value)
    {
        var cache = new TestCache();
        using var cancellation = new CancellationTokenSource();

        var missing = await cache.TryGetJsonAsync<int>("key");
        await cache.SetJsonAsync("key", value);
        var hit = await cache.TryGetJsonAsync<int>("key", cancellationToken: cancellation.Token);

        Assert.False(missing.Found);
        Assert.Equal(0, missing.Value);
        Assert.True(hit.Found);
        Assert.Equal(value, hit.Value);
        Assert.Equal(cancellation.Token, cache.LastGetToken);
    }

    [Fact]
    public async Task TryGet_Distinguishes_Cached_False_From_Missing_Values()
    {
        var cache = new TestCache();
        var missing = await cache.TryGetJsonAsync<bool>("key");
        await cache.SetJsonAsync("key", false);

        var hit = await cache.TryGetJsonAsync<bool>("key");

        Assert.False(missing.Found);
        Assert.False(missing.Value);
        Assert.True(hit.Found);
        Assert.False(hit.Value);
    }

    [Fact]
    public async Task TryGet_Reports_Json_Null_As_A_Hit()
    {
        var cache = new TestCache();
        cache.Set("key", Encoding.UTF8.GetBytes("null"), new());

        var missing = await cache.TryGetJsonAsync<string>("missing");
        var hit = await cache.TryGetJsonAsync<string>("key");

        Assert.False(missing.Found);
        Assert.Null(missing.Value);
        Assert.True(hit.Found);
        Assert.Null(hit.Value);
    }

    [Fact]
    public async Task TryGet_Treats_Empty_Bytes_As_A_Miss()
    {
        var cache = new TestCache();
        cache.Set("key", [], new());

        Assert.False((await cache.TryGetJsonAsync<string>("key")).Found);
    }

    [Fact]
    public async Task TryGet_Reports_Invalid_Json()
    {
        var cache = new TestCache();
        cache.Set("key", Encoding.UTF8.GetBytes("invalid json"), new());

        await Assert.ThrowsAsync<JsonException>(() => cache.TryGetJsonAsync<string>("key"));
    }

    [Fact]
    public async Task TryGet_Propagates_Cancellation_After_Cache_Read()
    {
        using var cancellation = new CancellationTokenSource();
        var cache = new TestCache { OnGet = cancellation.Cancel };
        await cache.SetJsonAsync("key", "value");

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            cache.TryGetJsonAsync<string>("key", cancellationToken: cancellation.Token));
    }

    [Fact]
    public async Task RemoveMany_Deduplicates_Keys_Using_Ordinal_Comparison()
    {
        var cache = new TestCache();
        using var cancellation = new CancellationTokenSource();
        foreach (var key in new[] { "key", "KEY", "other" }) await cache.SetJsonAsync(key, "value");

        await cache.RemoveManyAsync(["key", "KEY", "key", "other"], cancellation.Token);

        Assert.Equal(new[] { "KEY", "key", "other" }, cache.Removed.Order(StringComparer.Ordinal));
        Assert.Empty(cache.Values);
        Assert.Equal(cancellation.Token, cache.LastRemoveToken);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RemoveMany_Validates_All_Keys_Before_Deleting(string? invalidKey)
    {
        var cache = new TestCache();
        await cache.SetJsonAsync("valid", "value");

        await Assert.ThrowsAnyAsync<ArgumentException>(() => cache.RemoveManyAsync(["valid", invalidKey!]));

        Assert.Empty(cache.Removed);
        Assert.True(cache.Values.ContainsKey("valid"));
    }

    [Fact]
    public async Task RemoveMany_Allows_An_Empty_Collection()
    {
        var cache = new TestCache();

        await cache.RemoveManyAsync([]);

        Assert.Empty(cache.Removed);
    }

    [Fact]
    public async Task RemoveMany_Stops_On_Failure_Without_Undoing_Previous_Deletions()
    {
        var failure = new IOException("Cache unavailable.");
        var attempts = 0;
        var cache = new TestCache
        {
            OnRemove = _ =>
            {
                if (++attempts == 2) throw failure;
            }
        };
        foreach (var key in new[] { "first", "second", "third" }) await cache.SetJsonAsync(key, "value");

        var actual = await Assert.ThrowsAsync<IOException>(() => cache.RemoveManyAsync(["first", "second", "third"]));

        Assert.Same(failure, actual);
        Assert.Equal(2, attempts);
        Assert.Single(cache.Removed);
        Assert.Equal(2, cache.Values.Count);
    }

    [Fact]
    public async Task RemoveMany_Stops_When_Canceled_Between_Deletions()
    {
        using var cancellation = new CancellationTokenSource();
        var cache = new TestCache { OnRemove = _ => cancellation.Cancel() };
        foreach (var key in new[] { "first", "second" }) await cache.SetJsonAsync(key, "value");

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            cache.RemoveManyAsync(["first", "second"], cancellation.Token));

        Assert.Single(cache.Removed);
        Assert.Single(cache.Values);
    }

    public sealed record Entry(string Name);

    private sealed class TestCache : IDistributedCache
    {
        public Dictionary<string, byte[]> Values { get; } = [];
        public DistributedCacheEntryOptions? LastOptions { get; private set; }
        public CancellationToken LastGetToken { get; private set; }
        public CancellationToken LastSetToken { get; private set; }
        public CancellationToken LastRemoveToken { get; private set; }
        public List<string> Removed { get; } = [];
        public Action? OnGet { get; init; }
        public Action<string>? OnRemove { get; init; }
        public int GetCalls { get; private set; }
        public Exception? GetFailure { get; init; }
        public Exception? SetFailure { get; init; }

        public byte[]? Get(string key) => Values.GetValueOrDefault(key);

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            GetCalls++;
            LastGetToken = token;
            OnGet?.Invoke();
            return GetFailure is null ? Task.FromResult(Get(key)) : Task.FromException<byte[]?>(GetFailure);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            Values[key] = value;
            LastOptions = options;
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            LastSetToken = token;
            if (SetFailure is not null) return Task.FromException(SetFailure);
            Set(key, value, options);
            return Task.CompletedTask;
        }

        public void Refresh(string key) { }
        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;
        public void Remove(string key)
        {
            OnRemove?.Invoke(key);
            Values.Remove(key);
            Removed.Add(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            LastRemoveToken = token;
            Remove(key);
            return Task.CompletedTask;
        }
    }
}
