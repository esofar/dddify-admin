namespace Dddify.Admin.Web.Authorization;

/// <summary>
/// 声明访问控制器或接口所需的权限标识。
/// </summary>
/// <param name="code">权限标识。</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute(string code) : Attribute
{
    /// <summary>
    /// 权限标识。
    /// </summary>
    public string Code { get; } = code;
}
