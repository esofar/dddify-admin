namespace Dddify.Admin.Host.Models.Tokens;

public class ExchangeTokenRequest
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; }
}