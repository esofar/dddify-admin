using Dddify.Admin.Application.Commands.Auths;
using Dddify.Admin.Web.Requests.Auths;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 用户认证。
/// </summary>
/// <param name="sender">请求发送器。</param>
/// <param name="clock">系统时钟。</param>
/// <param name="currentUser">当前登录用户。</param>
[Route("api/v1/auth")]
public class AuthController(
    ISender sender,
    IClock clock,
    ICurrentUser currentUser) : BaseController
{
    /// <summary>
    /// 邮箱密码登录。
    /// </summary>
    /// <param name="request">邮箱密码登录请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>访问令牌。</returns>
    /// <remarks>刷新令牌写入 HttpOnly Cookie。</remarks>
    [AllowAnonymous]
    [HttpPost("login/password", Name = "LoginWithPassword")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> LoginWithPasswordAsync([FromBody] PasswordLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginWithPasswordCommand(
                request.Email,
                request.Password,
                request.DeviceId,
                request.DeviceName,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                request.IsPersistent),
            cancellationToken);

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, result.RefreshTokenDays, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 邮箱验证码登录。
    /// </summary>
    /// <param name="request">邮箱验证码登录请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>访问令牌。</returns>
    /// <remarks>刷新令牌写入 HttpOnly Cookie。</remarks>
    [AllowAnonymous]
    [HttpPost("login/verification-code", Name = "LoginWithVerificationCode")]
    [ProducesResponseType<ApiResult<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<string> LoginWithVerificationCodeAsync([FromBody] VerificationCodeLoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginWithVerificationCodeCommand(
                request.Email,
                request.Code,
                request.DeviceId,
                request.DeviceName,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                request.IsPersistent),
            cancellationToken);

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, result.RefreshTokenDays, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 刷新访问令牌并轮换刷新令牌。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>访问令牌。</returns>
    /// <remarks>需携带刷新令牌 Cookie 和 X-Device-Id 请求头。</remarks>
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

        HttpContext.AppendRefreshTokenCookie(result.RefreshToken, result.IsPersistent, result.RefreshTokenDays, clock);

        return result.AccessToken;
    }

    /// <summary>
    /// 退出当前设备登录并清除刷新令牌 Cookie。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
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
    /// 退出所有设备登录并清除当前 Cookie。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
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
    /// 发送邮箱登录验证码。
    /// </summary>
    /// <param name="request">发送邮箱登录验证码请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [AllowAnonymous]
    [HttpPost("login/verification-code/send", Name = "SendEmailVerificationCode")]
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
