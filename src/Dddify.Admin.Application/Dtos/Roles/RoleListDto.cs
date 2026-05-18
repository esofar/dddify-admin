namespace Dddify.Admin.Application.Dtos.Roles;

/// <summary>
/// 角色列表项。
/// </summary>
/// <param name="Id">角色 ID。</param>
/// <param name="Name">角色名称。</param>
/// <param name="Description">角色描述。</param>
/// <param name="IsPreset">是否为内置角色。</param>
/// <param name="IsDefault">是否为默认角色。</param>
/// <param name="AssignedUserCount">已分配用户数。</param>
/// <param name="Order">排序值。</param>
public record RoleListDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsPreset,
    bool IsDefault,
    int AssignedUserCount,
    int Order);
