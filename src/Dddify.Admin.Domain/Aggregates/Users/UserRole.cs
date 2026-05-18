namespace Dddify.Admin.Domain.Aggregates.Users;

/// <summary>
/// 用户与角色的关联实体。
/// </summary>
public class UserRole : Entity
{
    /// <summary>
    /// 角色名称最大长度。
    /// </summary>
    public const int MaxRoleNameLength = 50;

    /// <summary>
    /// 用户 ID。
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 角色 ID。
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 角色名称。
    /// </summary>
    public string RoleName { get; private set; } = default!;

    /// <summary>
    /// 是否为当前使用角色。
    /// </summary>
    public bool IsCurrent { get; private set; } = false;

    private UserRole() { }

    /// <summary>
    /// 创建用户角色关联。
    /// </summary>
    /// <param name="userId">用户 ID。</param>
    /// <param name="roleId">角色 ID。</param>
    /// <param name="roleName">角色名称。</param>
    public UserRole(Guid userId, Guid roleId, string roleName)
    {
        UserId = userId;
        RoleId = roleId;
        RoleName = roleName;
    }

    /// <summary>
    /// 修改角色名称快照。
    /// </summary>
    /// <param name="roleName">新的角色名称。</param>
    public void Rename(string roleName)
    {
        RoleName = roleName;
    }

    /// <summary>
    /// 设置是否为当前使用角色。
    /// </summary>
    /// <param name="isCurrent">是否为当前使用角色。</param>
    public void SetCurrent(bool isCurrent)
    {
        IsCurrent = isCurrent;
    }

    /// <summary>
    /// 获取用户角色关联实体的复合主键。
    /// </summary>
    /// <returns>由用户 ID 和角色 ID 组成的主键数组。</returns>
    public override object[] GetKeys()
    {
        return [UserId, RoleId];
    }
}
