namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 默认要求身份认证的 API 基础控制器。
/// </summary>
[Authorize]
[ApiController]
public class BaseController : ControllerBase
{
}
