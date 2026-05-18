namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// API 基础控制器，需要登录认证后才能访问。
/// </summary>
[Authorize]
[ApiController]
public class BaseController : ControllerBase
{
}