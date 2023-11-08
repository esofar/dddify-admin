namespace Dddify.Admin.Host.Models.Roles;

public class UpdateRoleRequest
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 并发标识
    /// </summary>
    public string ConcurrencyStamp { get; set; }
}