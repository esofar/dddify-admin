namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/captchas")]
public class CaptchaController : BaseController
{
    /// <summary>
    /// ��ȡ�˻���¼��֤��
    /// </summary>
    /// <param name="phone">�ֻ���</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult GetLoginCaptchaAsync([FromQuery] string phone)
    {
        return Ok("888888");
    }

    /// <summary>
    /// ��ȡ����������֤��
    /// </summary>
    /// <returns></returns>
    [HttpGet("password-reset")]
    public IActionResult GetPasswordResetCaptchaAsync()
    {
        return Ok("888888");
    }
}