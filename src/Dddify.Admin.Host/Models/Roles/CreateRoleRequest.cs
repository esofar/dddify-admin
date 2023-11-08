namespace Dddify.Admin.Host.Models.Roles;

public class CreateRoleRequest
{
    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }
}