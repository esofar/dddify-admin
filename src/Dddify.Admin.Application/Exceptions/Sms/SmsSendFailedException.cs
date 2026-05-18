namespace Dddify.Admin.Application.Exceptions.Sms;

public class SmsSendFailedException : AppException
{
    public SmsSendFailedException(string provider, string? code, string? message)
    {
        WithErrorCode("sms_send_failed");
        WithMetadata("Provider", provider);

        if (!string.IsNullOrWhiteSpace(code))
        {
            WithMetadata("Code", code);
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            WithMetadata("Message", message);
        }
    }
}
