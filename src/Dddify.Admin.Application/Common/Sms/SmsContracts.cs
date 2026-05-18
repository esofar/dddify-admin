namespace Dddify.Admin.Application.Common.Sms;

public sealed record SmsMessage(
    SmsScene Scene,
    string PhoneNumber,
    string TemplateName,
    IReadOnlyDictionary<string, string> Parameters,
    string RequestId,
    string TraceId);

public sealed record SmsSendResult(
    bool Succeeded,
    string? Provider,
    string? ProviderMessageId,
    string? ErrorCode,
    string? ErrorMessage);

public sealed record SmsProviderRequest(
    string PhoneNumber,
    string SignName,
    string TemplateCode,
    string TemplateContent,
    IReadOnlyDictionary<string, string> Parameters,
    IReadOnlyList<string> ParameterOrder,
    SmsScene Scene,
    string RequestId,
    string TraceId);

public sealed record SmsProviderResult(
    bool Succeeded,
    string? ProviderMessageId,
    string? ErrorCode,
    string? ErrorMessage,
    bool IsRetryable,
    bool CanFallback)
{
    public static SmsProviderResult Success(string? providerMessageId) =>
        new(true, providerMessageId, null, null, false, false);

    public static SmsProviderResult Failed(string? code, string? message, bool isRetryable, bool canFallback) =>
        new(false, null, code, message, isRetryable, canFallback);
}

public sealed record SmsRateLimitContext(
    SmsScene Scene,
    string PhoneNumber,
    string? IpAddress);

public sealed record SmsRateLimitResult(
    bool Allowed,
    string? Policy,
    TimeSpan? RetryAfter)
{
    public static SmsRateLimitResult AllowedResult { get; } = new(true, null, null);

    public static SmsRateLimitResult Rejected(string policy, TimeSpan retryAfter) =>
        new(false, policy, retryAfter);
}
