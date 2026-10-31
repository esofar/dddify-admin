namespace Dddify.Admin.Application.Abstractions.Email;

/// <summary>
/// 邮箱登录验证码服务。
/// </summary>
public interface IEmailVerificationCodeService
{
    /// <summary>
    /// 创建并保存验证码，受发送频率限制。
    /// </summary>
    /// <param name="email">邮箱地址。</param>
    /// <param name="ipAddress">来源 IP，用于限流。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>验证码及有效期。</returns>
    Task<GeneratedEmailVerificationCode> CreateAsync(string email, string? ipAddress, CancellationToken cancellationToken);

    /// <summary>
    /// 校验验证码，成功后立即消费。
    /// </summary>
    /// <param name="email">邮箱地址。</param>
    /// <param name="code">用户提交的验证码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>是否校验成功。</returns>
    /// <remarks>失败次数达到上限后使验证码失效。</remarks>
    Task<bool> VerifyAsync(string email, string code, CancellationToken cancellationToken);

    /// <summary>
    /// 删除匹配的验证码。
    /// </summary>
    /// <param name="email">邮箱地址。</param>
    /// <param name="code">待删除的验证码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task RemoveAsync(string email, string code, CancellationToken cancellationToken);
}

/// <summary>
/// 邮箱验证码生成结果。
/// </summary>
/// <param name="Value">六位数字验证码。</param>
/// <param name="ExpiresMinutes">有效期，单位为分钟。</param>
public sealed record GeneratedEmailVerificationCode(string Value, int ExpiresMinutes);
