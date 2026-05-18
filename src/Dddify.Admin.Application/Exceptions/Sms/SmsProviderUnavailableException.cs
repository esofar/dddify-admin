namespace Dddify.Admin.Application.Exceptions.Sms;

public class SmsProviderUnavailableException : AppException
{
    public SmsProviderUnavailableException(string provider)
    {
        WithErrorCode("sms_provider_unavailable");
        WithMetadata("Provider", provider);
    }
}
