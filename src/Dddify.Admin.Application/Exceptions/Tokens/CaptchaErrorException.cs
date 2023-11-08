namespace Dddify.Admin.Application.Exceptions.Tokens;

public class CaptchaErrorException : AppException
{
    public override string Name => "captcha_error";

    public CaptchaErrorException(string phoneNumber, string captcha)
        : base($"User with phone number '{phoneNumber}' captcha '{captcha}' is not correct.")
    {
    }
}