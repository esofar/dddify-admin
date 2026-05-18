namespace Dddify.Admin.Application.Exceptions.Sms;

public class SmsTemplateNotFoundException : AppException
{
    public SmsTemplateNotFoundException(string templateName)
    {
        WithErrorCode("sms_template_not_found");
        WithMetadata("TemplateName", templateName);
    }
}
