using StackExchange.Redis;

namespace Dddify.Admin.Infrastructure.Sms;

public sealed class RedisSmsRateLimiter(
    IConnectionMultiplexer connectionMultiplexer,
    IOptions<SmsOptions> options) : ISmsRateLimiter
{
    public async Task<SmsRateLimitResult> CheckAsync(SmsRateLimitContext context, CancellationToken cancellationToken)
    {
        if (!options.Value.RateLimit.Enabled)
        {
            return SmsRateLimitResult.AllowedResult;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var phoneHash = PhoneNumberProtector.Hash(context.PhoneNumber);
        var database = connectionMultiplexer.GetDatabase();

        var result = await CheckLimitAsync(
            database,
            SmsRedisKeys.Cooldown(context.Scene, phoneHash),
            1,
            TimeSpan.FromSeconds(options.Value.Verification.CooldownSeconds),
            "phone_cooldown");

        if (!result.Allowed)
        {
            return result;
        }

        result = await CheckLimitAsync(
            database,
            SmsRedisKeys.PhoneLimit(context.Scene, phoneHash, options.Value.RateLimit.PerPhoneWindowSeconds),
            options.Value.RateLimit.MaxSendsPerPhone,
            TimeSpan.FromSeconds(options.Value.RateLimit.PerPhoneWindowSeconds),
            "phone_window");

        if (!result.Allowed)
        {
            return result;
        }

        result = await CheckLimitAsync(
            database,
            SmsRedisKeys.PhoneLimit(context.Scene, phoneHash, options.Value.Verification.SendWindowMinutes * 60),
            options.Value.Verification.MaxSendsPerWindow,
            TimeSpan.FromMinutes(options.Value.Verification.SendWindowMinutes),
            "scene_phone_window");

        if (!result.Allowed)
        {
            return result;
        }

        if (!string.IsNullOrWhiteSpace(context.IpAddress))
        {
            result = await CheckLimitAsync(
                database,
                SmsRedisKeys.IpLimit(context.Scene, PhoneNumberProtector.Hash(context.IpAddress), options.Value.RateLimit.PerIpWindowSeconds),
                options.Value.RateLimit.MaxSendsPerIp,
                TimeSpan.FromSeconds(options.Value.RateLimit.PerIpWindowSeconds),
                "ip_window");
        }

        return result;
    }

    private static async Task<SmsRateLimitResult> CheckLimitAsync(IDatabase database, RedisKey key, int limit, TimeSpan window, string policy)
    {
        var count = await database.StringIncrementAsync(key);

        if (count == 1)
        {
            await database.KeyExpireAsync(key, window);
        }

        if (count > limit)
        {
            var retryAfter = await database.KeyTimeToLiveAsync(key);

            return SmsRateLimitResult.Rejected(policy, retryAfter ?? window);
        }

        return SmsRateLimitResult.AllowedResult;
    }
}
