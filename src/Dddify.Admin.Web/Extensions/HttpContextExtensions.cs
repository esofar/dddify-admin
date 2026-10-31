namespace Dddify.Admin.Web.Extensions;

/// <summary>
/// HTTP 请求信息扩展。
/// </summary>
public static class HttpContextExtensions
{
    private const string DeviceIdHeaderName = "X-Device-Id";

    /// <summary>
    /// 获取客户端 IP，并将映射的 IPv4 地址转为 IPv4 格式。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文。</param>
    /// <returns>客户端 IP，无法获取时为 null。</returns>
    /// <remarks>代理转发由 ForwardedHeaders 中间件处理。</remarks>
    public static string? GetClientIpAddress(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        var address = httpContext.Connection.RemoteIpAddress;
        return address?.IsIPv4MappedToIPv6 == true ? address.MapToIPv4().ToString() : address?.ToString();
    }

    /// <summary>
    /// 读取 User-Agent 请求头。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文。</param>
    /// <returns>用户代理，未提供时为空字符串。</returns>
    public static string GetUserAgent(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        return httpContext.Request.Headers.UserAgent.ToString();
    }

    /// <summary>
    /// 读取 X-Device-Id 请求头。
    /// </summary>
    /// <param name="httpContext">HTTP 上下文。</param>
    /// <returns>设备 ID，未提供时为空字符串。</returns>
    public static string GetDeviceId(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        return httpContext.Request.Headers[DeviceIdHeaderName].ToString();
    }
}
