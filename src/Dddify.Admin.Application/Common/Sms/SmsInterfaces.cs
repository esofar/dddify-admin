namespace Dddify.Admin.Application.Common.Sms;

public interface ISmsProvider
{
    string Name { get; }

    Task<SmsProviderResult> SendAsync(SmsProviderRequest request, CancellationToken cancellationToken);
}

public interface ISmsSender
{
    Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken);
}

public interface ISmsVerificationCodeService
{
    string GenerateCode(int length = 6);

    Task StoreAsync(
        SmsScene scene,
        string phoneNumber,
        string code,
        TimeSpan expiresIn,
        TimeSpan verifyWindow,
        CancellationToken cancellationToken);

    Task<bool> VerifyAsync(
        SmsScene scene,
        string phoneNumber,
        string code,
        int maxAttempts,
        TimeSpan verifyWindow,
        CancellationToken cancellationToken);
}

public interface ISmsRateLimiter
{
    Task<SmsRateLimitResult> CheckAsync(SmsRateLimitContext context, CancellationToken cancellationToken);
}
