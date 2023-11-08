namespace Dddify.Admin.Domain.Constants;

/// <summary>
/// 正则匹配字符串
/// </summary>
public class RegexStrings
{
    /// <summary>
    /// 编码
    /// </summary>
    public const string Code = "^[a-z_-]{3,15}$";

    /// <summary>
    /// 中国大陆手机号
    /// </summary>
    public const string PhoneNumber = "^1(3\\d|4[5-9]|5[0-35-9]|6[2567]|7[0-8]|8\\d|9[0-35-9])\\d{8}$";
}