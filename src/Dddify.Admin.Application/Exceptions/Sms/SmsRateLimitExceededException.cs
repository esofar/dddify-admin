namespace Dddify.Admin.Application.Exceptions.Sms;

public class SmsRateLimitExceededException : AppException
{
    public SmsRateLimitExceededException(string policy, TimeSpan window)
    {
        WithErrorCode("sms_rate_limit_exceeded");
        WithMetadata("Policy", policy);
        WithMetadata("RetryAfterSeconds", (int)Math.Ceiling(window.TotalSeconds));
    }
}
