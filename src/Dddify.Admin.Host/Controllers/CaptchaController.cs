namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/captchas")]
public class CaptchaController : BaseController
{
    /// <summary>
    /// 获取账户登录验证码
    /// </summary>
    /// <param name="phone">手机号</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult GetLoginCaptchaAsync([FromQuery] string phone)
    {
        return Ok("888888");
    }

    /// <summary>
    /// 获取密码重置验证码
    /// </summary>
    /// <returns></returns>
    [HttpGet("password-reset")]
    public IActionResult GetPasswordResetCaptchaAsync()
    {
        return Ok("888888");
    }
}