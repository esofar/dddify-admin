namespace Dddify.Admin.Host.Models.Tokens;

public class GenerateTokenByPhoneRequest
{
    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    public string Captcha { get; set; }
}