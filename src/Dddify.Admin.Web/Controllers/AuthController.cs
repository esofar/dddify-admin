using Dddify.Admin.Application.Commands.Auths;
using Dddify.Admin.Web.Requests.Auths;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 认证相关。
/// </summary>
/// <param name="sender">请求发送器。</param>
/// <param name="clock">系统时钟。</param>
/// <param name="clientAccessor">客户端访问上下文。</param>
/// <param name="currentUser">当前登录用户。</param>
/// <param name="jwtOptions">JWT 配置选项。</param>
[Route("api/v1/auth")]
public class AuthController(
    ISender sender,
    IClock clock,
    IClientAccessor clientAccessor,
    ICurrentUser currentUser,
    IOptions<JwtOptions> jwtOptions) : BaseController
{
    private readonly JwtOptions jwt = jwtOptions.Value;

    /// <summary>
    /// 账号密码登录。
    /// </summary>
    /// <param name="request">账号密码登录请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login/account", Name = "AccountLogin")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> LoginAsync([FromBody] AccountLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AccountLoginCommand(
                request.Account,
                request.Password,
                request.DeviceId,
                request.DeviceName,
                clientAccessor.IpAddress,
                clientAccessor.UserAgent,
                request.RememberMe),
            cancellationToken);

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, jwt, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 手机验证码登录。
    /// </summary>
    /// <param name="request">手机验证码登录请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login/sms", Name = "SmsLogin")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> SmsLoginAsync([FromBody] SmsLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SmsLoginCommand(
                request.PhoneNumber,
                request.Code,
                request.DeviceId,
                request.DeviceName,
                clientAccessor.IpAddress,
                clientAccessor.UserAgent,
                request.RememberMe),
            cancellationToken);

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, jwt, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 刷新访问令牌。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("token/refresh", Name = "RefreshToken")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var refreshToken = HttpContext.GetRefreshTokenCookie();
        var deviceId = HttpContext.GetDeviceId();

        var result = await sender.Send(
            new RefreshTokenCommand(refreshToken, deviceId),
            cancellationToken);

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, jwt, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 退出登录。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost("logout", Name = "Logout")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        var deviceId = HttpContext.GetDeviceId();

        await sender.Send(
            new LogoutCommand(currentUser.GetIdAsGuid(), deviceId),
            cancellationToken);

        HttpContext.DeleteRefreshTokenCookie();
    }

    /// <summary>
    /// 所有设备退出登录。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost("logout/all", Name = "LogoutAll")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task LogoutAllAsync(CancellationToken cancellationToken)
    {
        await sender.Send(
            new LogoutCommand(currentUser.GetIdAsGuid()),
            cancellationToken);

        HttpContext.DeleteRefreshTokenCookie();
    }

    /// <summary>
    /// 发送登录短信验证码。
    /// </summary>
    /// <param name="request">发送登录短信验证码请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost("login/sms-code", Name = "SendLoginSmsCode")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status429TooManyRequests)]
    public async Task SendLoginSmsCodeAsync([FromBody] SendLoginSmsCodeRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new SendLoginSmsCodeCommand(
                request.PhoneNumber,
                clientAccessor.IpAddress),
            cancellationToken);
    }
}