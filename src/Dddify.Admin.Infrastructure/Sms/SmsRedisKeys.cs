namespace Dddify.Admin.Infrastructure.Sms;

/// <summary>
/// 短信业务 Redis Key 定义。
/// </summary>
/// <remarks>
/// Key 设计原则：
/// 1. 按业务场景（Scene）隔离，避免不同短信场景互相影响。
/// 2. 手机号、IP 等敏感信息必须先 Hash 后再作为 Key 参数。
/// 3. Key 命名统一使用 sms:* 前缀，便于运维排查与监控。
/// 4. 验证码、限流、错误次数分别使用独立 Key，避免职责混乱。
/// </remarks>
internal static class SmsRedisKeys
{
    /// <summary>
    /// 验证码存储 Key。
    /// </summary>
    /// <param name="scene">短信业务场景，例如 Login/Register。</param>
    /// <param name="phoneHash">手机号 Hash。</param>
    /// <returns>验证码 Redis Key。</returns>
    public static string Code(SmsScene scene, string phoneHash) =>
        $"sms:code:{scene}:{phoneHash}";

    /// <summary>
    /// 验证码错误次数统计 Key。
    /// </summary>
    /// <remarks>
    /// 用于限制验证码连续输错次数，防止暴力破解。
    /// 超过最大错误次数后，验证码可直接失效。
    /// </remarks>
    /// <param name="scene">短信业务场景。</param>
    /// <param name="phoneHash">手机号 Hash。</param>
    /// <returns>验证码错误次数 Redis Key。</returns>
    public static string VerifyAttempts(SmsScene scene, string phoneHash) =>
        $"sms:code:attempts:{scene}:{phoneHash}";

    /// <summary>
    /// 短信发送冷却时间 Key。
    /// </summary>
    /// <remarks>
    /// 用于限制同一手机号短时间内重复发送验证码。
    /// 例如 60 秒内只能发送一次。
    /// </remarks>
    /// <param name="scene">短信业务场景。</param>
    /// <param name="phoneHash">手机号 Hash。</param>
    /// <returns>短信发送冷却 Redis Key。</returns>
    public static string Cooldown(SmsScene scene, string phoneHash) =>
        $"sms:cooldown:{scene}:{phoneHash}";

    /// <summary>
    /// 手机号维度限流 Key。
    /// </summary>
    /// <remarks>
    /// 用于限制同一手机号在指定时间窗口内的短信发送次数。
    /// </remarks>
    /// <param name="scene">短信业务场景。</param>
    /// <param name="phoneHash">手机号 Hash。</param>
    /// <param name="windowSeconds">限流时间窗口（秒）。</param>
    /// <returns>手机号限流 Redis Key。</returns>
    public static string PhoneLimit(SmsScene scene, string phoneHash, int windowSeconds) =>
        $"sms:limit:phone:{scene}:{phoneHash}:{windowSeconds}s";

    /// <summary>
    /// IP 维度限流 Key。
    /// </summary>
    /// <remarks>
    /// 用于限制同一 IP 在指定时间窗口内的短信发送次数，
    /// 防止短信轰炸与恶意刷接口。
    /// </remarks>
    /// <param name="scene">短信业务场景。</param>
    /// <param name="ipHash">IP Hash。</param>
    /// <param name="windowSeconds">限流时间窗口（秒）。</param>
    /// <returns>IP 限流 Redis Key。</returns>
    public static string IpLimit(SmsScene scene, string ipHash, int windowSeconds) =>
        $"sms:limit:ip:{scene}:{ipHash}:{windowSeconds}s";
}
