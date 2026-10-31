namespace Dddify.Admin.Web.Authentication;

/// <summary>
/// 刷新令牌 Cookie 扩展。
/// </summary>
public static class RefreshTokenCookieExtensions
{
    private const string RefreshTokenCookieName = "__Host-refresh_token";
    private const string RefreshTokenCookiePath = "/";
    private const string DeviceIdHeaderName = "X-Device-Id";

    /// <summary>
    /// 读取刷新令牌 Cookie。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文。</param>
    /// <returns>令牌明文，不存在时为空字符串。</returns>
    public static string GetRefreshTokenCookie(this HttpContext httpContext)
    {
        return httpContext.Request.Cookies[RefreshTokenCookieName] ?? string.Empty;
    }

    /// <summary>
    /// 获取设备ID。
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static string GetDeviceId(this HttpContext httpContext)
    {
        return httpContext.Request.Headers.TryGetValue(DeviceIdHeaderName, out var value)
            ? value.ToString()
            : string.Empty;
    }

    /// <summary>
    /// 写入 HttpOnly 刷新令牌 Cookie。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文。</param>
    /// <param name="refreshToken">刷新令牌明文。</param>
    /// <param name="persistent">是否持久化；为 false 时使用会话 Cookie。</param>
    /// <param name="refreshTokenDays">刷新令牌有效天数。</param>
    /// <param name="clock">系统时钟。</param>
    public static void AppendRefreshTokenCookie(
        this HttpContext httpContext,
        string refreshToken,
        bool persistent,
        int refreshTokenDays,
        IClock clock)
    {
        var options = CreateRefreshTokenCookieOptions();

        if (persistent)
        {
            options.MaxAge = TimeSpan.FromDays(refreshTokenDays);
            options.Expires = clock.UtcNow.AddDays(refreshTokenDays);
        }

        httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
    }

    /// <summary>
    /// 删除刷新令牌 Cookie。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文。</param>
    public static void DeleteRefreshTokenCookie(this HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(RefreshTokenCookieName, CreateRefreshTokenCookieOptions());
    }

    /// <summary>
    /// 创建刷新令牌 Cookie 安全选项。
    /// </summary>
    /// <returns>Cookie 安全选项。</returns>
    private static CookieOptions CreateRefreshTokenCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Path = RefreshTokenCookiePath,
        };
    }
}
