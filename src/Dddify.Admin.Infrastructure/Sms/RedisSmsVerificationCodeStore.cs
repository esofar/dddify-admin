using StackExchange.Redis;
using System.Security.Cryptography;

namespace Dddify.Admin.Infrastructure.Sms;

public sealed class RedisSmsVerificationCodeService(
    IConnectionMultiplexer connectionMultiplexer,
    IOptions<SmsOptions> options) : ISmsVerificationCodeService
{
    private const string VerifyScript = """
        local code = redis.call('GET', KEYS[1])
        if not code then
            return 0
        end

        local attempts = tonumber(redis.call('GET', KEYS[2]) or '0')
        if attempts >= tonumber(ARGV[2]) then
            return 0
        end

        if code == ARGV[1] then
            redis.call('DEL', KEYS[1])
            redis.call('DEL', KEYS[2])
            return 1
        end

        attempts = redis.call('INCR', KEYS[2])
        if attempts == 1 then
            redis.call('PEXPIRE', KEYS[2], ARGV[3])
        end

        return 0
        """;

    private const int MaxCodeLength = 32;

    public string GenerateCode(int length = 6)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Verification code length must be greater than 0.");
        }

        if (length > MaxCodeLength)
        {
            throw new ArgumentOutOfRangeException(nameof(length), $"Verification code length cannot exceed {MaxCodeLength}.");
        }

        Span<char> code = stackalloc char[length];

        for (var i = 0; i < length; i++)
        {
            code[i] = (char)('0' + RandomNumberGenerator.GetInt32(0, 10));
        }

        return new string(code);
    }

    public async Task StoreAsync(
        SmsScene scene,
        string phoneNumber,
        string code,
        TimeSpan expiresIn,
        TimeSpan verifyWindow,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var database = connectionMultiplexer.GetDatabase();
        var phoneHash = PhoneNumberProtector.Hash(phoneNumber);

        await database.StringSetAsync(
            SmsRedisKeys.Code(scene, phoneHash),
            HashCode(scene, phoneNumber, code),
            expiresIn);

        await database.KeyDeleteAsync(SmsRedisKeys.VerifyAttempts(scene, phoneHash));
    }

    public async Task<bool> VerifyAsync(
        SmsScene scene,
        string phoneNumber,
        string code,
        int maxAttempts,
        TimeSpan verifyWindow,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var database = connectionMultiplexer.GetDatabase();
        var phoneHash = PhoneNumberProtector.Hash(phoneNumber);
        var verifyWindowMilliseconds = Math.Max(1, (long)Math.Ceiling(verifyWindow.TotalMilliseconds));

        var result = (int)await database.ScriptEvaluateAsync(
            VerifyScript,
            [SmsRedisKeys.Code(scene, phoneHash), SmsRedisKeys.VerifyAttempts(scene, phoneHash)],
            [HashCode(scene, phoneNumber, code), maxAttempts, verifyWindowMilliseconds]);

        return result == 1;
    }

    private string HashCode(SmsScene scene, string phoneNumber, string code)
    {
        var secret = options.Value.Verification.HashSecret;

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("Sms:Verification:HashSecret cannot be empty.");
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes($"{scene}:{phoneNumber}:{code}"));

        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
