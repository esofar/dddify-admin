namespace Dddify.Admin.Application.Exceptions.Email;

public class EmailVerificationCodeRateLimitExceededException : AppException
{
    public EmailVerificationCodeRateLimitExceededException(TimeSpan retryAfter)
    {
        WithErrorCode("email_code_rate_limit_exceeded");
        WithMetadata("RetryAfterSeconds", (int)Math.Ceiling(retryAfter.TotalSeconds));
    }
}
