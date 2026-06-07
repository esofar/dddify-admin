namespace Dddify.Admin.Web.Authentication;

/// <summary>
/// Refresh Token Cookie 扩展方法。
/// </summary>
public static class RefreshTokenCookieExtensions
{
    private const string RefreshTokenCookieName = "__Host-refresh_token";
    private const string RefreshTokenCookiePath = "/";
    private const string DeviceIdHeaderName = "X-Device-Id";

    /// <summary>
    /// 从请求 Cookie 中读取 Refresh Token 明文。
    /// </summary>
    /// <param name="httpContext">当前 HTTP 上下文。</param>
    /// <returns>Refresh Token 明文；不存在时返回空字符串。</returns>
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
    /// 写入 Refresh Token Cookie。
    /// </summary>
    /// <param name="httpContext">当前 HTTP 上下文。</param>
    /// <param name="refreshToken">Refresh Token 明文，仅写入 HttpOnly Cookie。</param>
    /// <param name="persistent">是否持久化 Cookie；为 false 时创建浏览器会话 Cookie。</param>
    /// <param name="jwtOptions">JWT 配置，用于获取 Refresh Token 有效期。</param>
    /// <param name="clock">系统时钟。</param>
    public static void AppendRefreshTokenCookie(
        this HttpContext httpContext,
        string refreshToken,
        bool persistent,
        JwtOptions jwtOptions,
        IClock clock)
    {
        var options = CreateRefreshTokenCookieOptions();

        if (persistent)
        {
            options.MaxAge = TimeSpan.FromDays(jwtOptions.RefreshTokenDays);
            options.Expires = clock.UtcNow.AddDays(jwtOptions.RefreshTokenDays);
        }

        httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
    }

    /// <summary>
    /// 删除 Refresh Token Cookie。
    /// </summary>
    /// <param name="httpContext">当前 HTTP 上下文。</param>
    public static void DeleteRefreshTokenCookie(this HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(RefreshTokenCookieName, CreateRefreshTokenCookieOptions());
    }

    /// <summary>
    /// 创建 Refresh Token Cookie 安全选项。
    /// </summary>
    /// <returns>Refresh Token Cookie 选项。</returns>
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
