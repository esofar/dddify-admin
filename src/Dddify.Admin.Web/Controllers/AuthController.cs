using Dddify.Admin.Application.Commands.Auths;
using Dddify.Admin.Web.Requests.Auths;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 认证相关。
/// </summary>
/// <param name="sender">请求发送器。</param>
/// <param name="clock">系统时钟。</param>
/// <param name="currentUser">当前登录用户。</param>
/// <param name="jwtOptions">JWT 配置选项。</param>
[Route("api/v1/auth")]
public class AuthController(
    ISender sender,
    IClock clock,
    ICurrentUser currentUser,
    IOptions<JwtOptions> jwtOptions) : BaseController
{
    private readonly JwtOptions jwt = jwtOptions.Value;

    /// <summary>
    /// 邮箱密码登录。
    /// </summary>
    /// <param name="request">邮箱密码登录请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login/account", Name = "PasswordLogin")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> PasswordLoginAsync([FromBody] PasswordLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new PasswordLoginCommand(
                request.Email,
                request.Password,
                request.DeviceId,
                request.DeviceName,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                request.RememberMe),
            cancellationToken);

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, jwt, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 邮箱验证码登录。
    /// </summary>
    /// <param name="request">邮箱验证码登录请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login/email-code", Name = "VerificationCodeLogin")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> VerificationCodeLoginAsync([FromBody] VerificationCodeLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new VerificationCodeLoginCommand(
                request.Email,
                request.Code,
                request.DeviceId,
                request.DeviceName,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
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
    /// 发送登录邮箱验证码。
    /// </summary>
    /// <param name="request">发送登录邮箱验证码请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login/email-code/send", Name = "SendEmailVerificationCode")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status429TooManyRequests)]
    public async Task SendEmailVerificationCodeAsync([FromBody] SendEmailVerificationCodeRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new SendEmailVerificationCodeCommand(
                request.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString()),
            cancellationToken);
    }
}
