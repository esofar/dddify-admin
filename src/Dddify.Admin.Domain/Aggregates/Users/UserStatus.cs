namespace Dddify.Admin.Domain.Aggregates.Users;

/// <summary>
/// 用户账号状态枚举。
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// 启用。
    /// </summary>
    Enabled = 1,

    /// <summary>
    /// 禁用。
    /// </summary>
    Disabled = 2,
}
