namespace Dddify.Admin.Web.Authorization;

/// <summary>
/// 声明访问控制器或接口方法所需的权限编码。
/// </summary>
/// <param name="code">权限编码。</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute(string code) : Attribute
{
    /// <summary>
    /// 权限编码。
    /// </summary>
    public string Code { get; } = code;
}
