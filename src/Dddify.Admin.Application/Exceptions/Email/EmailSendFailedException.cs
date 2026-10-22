namespace Dddify.Admin.Application.Exceptions.Email;

public class EmailSendFailedException : AppException
{
    public EmailSendFailedException()
    {
        WithErrorCode("email_send_failed");
    }
}
