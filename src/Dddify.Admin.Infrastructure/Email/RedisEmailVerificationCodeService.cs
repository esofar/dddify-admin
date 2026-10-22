using Dddify.Admin.Application.Exceptions.Email;
using Dddify.Admin.Application.Services.Email;
using Dddify.Admin.Infrastructure.Caching;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Email;

public sealed class RedisEmailVerificationCodeService(
    IConnectionMultiplexer connection,
    IOptions<EmailVerificationCodeOptions> options,
    IOptions<RedisOptions> redisOptions) : IEmailVerificationCodeService
{
    // Check every limit before mutating counters; all writes and expiries are atomic.
    private const string CreateScript = """
        local cooldown = redis.call('PTTL', KEYS[1])
        if cooldown > 0 then return cooldown end
        for i = 2, 3 do
            if tonumber(redis.call('GET', KEYS[i]) or '0') >= tonumber(ARGV[i]) then
                return math.max(1, redis.call('PTTL', KEYS[i]))
            end
        end
        redis.call('SET', KEYS[1], '1', 'PX', ARGV[1])
        for i = 2, 3 do
            if redis.call('INCR', KEYS[i]) == 1 then
                redis.call('PEXPIRE', KEYS[i], 3600000)
            end
        end
        redis.call('SET', KEYS[4], ARGV[4], 'PX', ARGV[5])
        redis.call('DEL', KEYS[5])
        return 0
        """;

    private const string VerifyScript = """
        local code = redis.call('GET', KEYS[1])
        if not code then return 0 end
        if code == ARGV[1] then
            redis.call('DEL', KEYS[1], KEYS[2])
            return 1
        end
        local attempts = redis.call('INCR', KEYS[2])
        if attempts == 1 then
            redis.call('PEXPIRE', KEYS[2], math.max(1, redis.call('PTTL', KEYS[1])))
        end
        if attempts >= tonumber(ARGV[2]) then
            redis.call('DEL', KEYS[1], KEYS[2])
        end
        return 0
        """;

    private const string RemoveScript = """
        if redis.call('GET', KEYS[1]) == ARGV[1] then
            redis.call('DEL', KEYS[1], KEYS[2])
        end
        return 0
        """;

    public async Task<string> CreateAsync(string email, string? ipAddress, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var settings = options.Value;
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var key = GetKey(email);
        var ipKey = GetKey(ipAddress ?? "unknown");
        var retryMilliseconds = (long)await connection.GetDatabase().ScriptEvaluateAsync(
            CreateScript,
            [$"{key}:cooldown", $"{key}:sends", $"{ipKey}:ip-sends", $"{key}:code", $"{key}:attempts"],
            [settings.CooldownSeconds * 1000, settings.MaxSendsPerEmailPerHour,
                settings.MaxSendsPerIpPerHour, HashCode(email, code), settings.ExpiresMinutes * 60_000]);

        if (retryMilliseconds > 0)
        {
            throw new EmailVerificationCodeRateLimitExceededException(TimeSpan.FromMilliseconds(retryMilliseconds));
        }

        return code;
    }

    public async Task<bool> VerifyAsync(string email, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var key = GetKey(email);
        var result = (int)await connection.GetDatabase().ScriptEvaluateAsync(
            VerifyScript,
            [$"{key}:code", $"{key}:attempts"],
            [HashCode(email, code), options.Value.MaxVerifyAttempts]);
        return result == 1;
    }

    public async Task RemoveAsync(string email, string code, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var key = GetKey(email);
        await connection.GetDatabase().ScriptEvaluateAsync(
            RemoveScript,
            [$"{key}:code", $"{key}:attempts"],
            [HashCode(email, code)]);
    }

    private string GetKey(string value)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value.Trim().ToLowerInvariant())));
        // A shared Redis hash tag keeps Lua keys in the same slot in a cluster.
        return $"{redisOptions.Value.InstanceName}{{email-login}}:{hash}";
    }

    private string HashCode(string email, string code)
    {
        var data = Encoding.UTF8.GetBytes($"login:{email.Trim().ToLowerInvariant()}:{code}");
        return Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(options.Value.HashSecret), data));
    }
}
