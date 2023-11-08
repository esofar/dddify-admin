namespace Dddify.Admin.Host.Models.Tokens;

public class GenerateTokenByEmailRequest
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
}