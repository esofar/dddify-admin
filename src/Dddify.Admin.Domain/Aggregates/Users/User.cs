using Dddify.Admin.Domain.Events.Users;
using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Domain.Aggregates.Users;

/// <summary>
/// 用户聚合根。
/// </summary>
public class User : AuditableAggregateRoot<Guid>
{
    /// <summary>
    /// 明文密码最大长度。
    /// </summary>
    public const int MaxPasswordLength = 50;

    /// <summary>
    /// 姓名最大长度。
    /// </summary>
    public const int MaxNameLength = 20;

    /// <summary>
    /// 昵称最大长度。
    /// </summary>
    public const int MaxNickNameLength = 20;

    /// <summary>
    /// 密码哈希最大长度。
    /// </summary>
    public const int MaxPasswordHashLength = 200;

    /// <summary>
    /// 头像地址最大长度。
    /// </summary>
    public const int MaxAvatarLength = 500;

    /// <summary>
    /// 邮箱最大长度。
    /// </summary>
    public const int MaxEmailLength = 254;

    /// <summary>
    /// 手机号最大长度。
    /// </summary>
    public const int MaxPhoneNumberLength = 11;

    /// <summary>
    /// 姓名格式正则表达式。
    /// </summary>
    public const string NamePattern = @"^[\u4e00-\u9fa5a-zA-Z0-9]+$";

    /// <summary>
    /// 昵称格式正则表达式。
    /// </summary>
    public const string NickNamePattern = @"^[\u4e00-\u9fa5a-zA-Z0-9_]+$";

    /// <summary>
    /// 密码格式正则表达式，要求至少包含大小写字母和数字。
    /// </summary>
    public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

    private readonly List<UserRole> _roles = [];

    /// <summary>
    /// 姓名。
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// 昵称。
    /// </summary>
    public string? NickName { get; private set; }

    /// <summary>
    /// 密码哈希。
    /// </summary>
    public string PasswordHash { get; private set; } = default!;

    /// <summary>
    /// 头像地址。
    /// </summary>
    public string? Avatar { get; private set; }

    /// <summary>
    /// 邮箱。
    /// </summary>
    public string Email { get; private set; } = default!;

    /// <summary>
    /// 手机号。
    /// </summary>
    public string PhoneNumber { get; private set; } = default!;

    /// <summary>
    /// 出生日期。
    /// </summary>
    public DateOnly? BirthDate { get; private set; }

    /// <summary>
    /// 性别。
    /// </summary>
    public UserGender Gender { get; private set; }

    /// <summary>
    /// 状态。
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// 最后登录时间。
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; private set; }

    /// <summary>
    /// 部门。
    /// </summary>
    public UserDepartment Department { get; private set; } = default!;

    /// <summary>
    /// 是否内置。
    /// </summary>
    public bool IsBuiltIn { get; private set; } = false;

    /// <summary>
    /// 已分配角色集合。
    /// </summary>
    /// <remarks>
    /// 返回只读集合，外部无法直接修改角色数据。
    /// 角色的增删应通过聚合根提供的领域行为进行操作。
    /// </remarks>
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    /// <summary>
    /// 并发标记。
    /// </summary>
    public string? ConcurrencyStamp { get; set; }

    private User() { }

    /// <summary>
    /// 创建用户。
    /// </summary>
    /// <param name="id">用户 ID。</param>
    /// <param name="passwordHash">密码哈希。</param>
    /// <param name="name">姓名。</param>
    /// <param name="nickName">昵称。</param>
    /// <param name="gender">性别。</param>
    /// <param name="birthDate">出生日期。</param>
    /// <param name="email">邮箱。</param>
    /// <param name="phoneNumber">手机号。</param>
    /// <param name="departmentId">部门 ID。</param>
    /// <param name="departmentName">部门名称。</param>
    public User(
        Guid id,
        string passwordHash,
        string name,
        string? nickName,
        UserGender gender,
        DateOnly? birthDate,
        string email,
        string phoneNumber,
        Guid departmentId,
        string departmentName)
    {
        Id = id;
        PasswordHash = passwordHash;
        Name = name;
        NickName = nickName;
        Gender = gender;
        BirthDate = birthDate;
        Email = email;
        PhoneNumber = phoneNumber;
        Department = new UserDepartment(departmentId, departmentName);
        Status = UserStatus.Enabled;
    }

    /// <summary>
    /// 修改用户基本信息。
    /// </summary>
    /// <param name="name">姓名。</param>
    /// <param name="nickName">昵称。</param>
    /// <param name="gender">性别。</param>
    /// <param name="birthDate">出生日期。</param>
    /// <param name="email">邮箱。</param>
    /// <param name="phoneNumber">手机号。</param>
    public void Change(
        string name,
        string? nickName,
        UserGender gender,
        DateOnly? birthDate,
        string email,
        string phoneNumber)
    {
        Name = name;
        NickName = nickName;
        Gender = gender;
        BirthDate = birthDate;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// 确保用户处于可用状态。
    /// </summary>
    /// <exception cref="UserAlreadyDisabledException">用户已被禁用时抛出。</exception>
    public void EnsureActive()
    {
        if (Status == UserStatus.Disabled)
        {
            throw new UserAlreadyDisabledException(Id);
        }
    }

    /// <summary>
    /// 标记用户登录成功，并登记登录成功领域事件。
    /// </summary>
    /// <param name="deviceId">设备 ID。</param>
    /// <param name="deviceName">设备名称。</param>
    /// <param name="ipAddress">登录 IP 地址。</param>
    /// <param name="userAgent">用户代理。</param>
    /// <param name="refreshTokenHash">刷新令牌哈希。</param>
    /// <param name="refreshTokenExpiresAt">刷新令牌过期时间。</param>
    /// <param name="loggedInAt">登录时间。</param>
    /// <param name="isPersistent">是否持久化登录状态。</param>
    public void MarkLoginSucceeded(
        string deviceId,
        string deviceName,
        string? ipAddress,
        string? userAgent,
        string refreshTokenHash,
        DateTimeOffset refreshTokenExpiresAt,
        DateTimeOffset loggedInAt,
        bool isPersistent)
    {
        LastLoginAt = loggedInAt;

        AddDomainEvent(new UserLoginSucceededDomainEvent(
            Id,
            deviceId,
            deviceName,
            ipAddress,
            userAgent,
            refreshTokenHash,
            refreshTokenExpiresAt,
            isPersistent));
    }

    /// <summary>
    /// 分配用户角色。
    /// </summary>
    /// <param name="roles">新的角色集合。</param>
    /// <exception cref="UserRolesEmptyException">角色集合为空时抛出。</exception>
    /// <remarks>
    /// 若原当前角色仍在新的角色集合中，则保持该角色为当前角色；
    /// 否则将新的角色集合中的第一个角色设置为当前角色。
    /// 角色发生增删时会登记用户角色变更领域事件。
    /// </remarks>
    public void AssignRoles(IEnumerable<UserRole> roles)
    {
        if (!roles.Any())
        {
            throw new UserRolesEmptyException(Id);
        }

        var oldRoleIds = _roles.Select(r => r.RoleId).ToHashSet();
        var newRoleIds = roles.Select(r => r.RoleId).ToHashSet();

        var added = newRoleIds.Except(oldRoleIds).ToList();
        var removed = oldRoleIds.Except(newRoleIds).ToList();

        var newUserRoles = roles.ToList();

        var currentRoleId = _roles.FirstOrDefault(r => r.IsCurrent)?.RoleId;
        var roleToSetCurrent = currentRoleId.HasValue && newUserRoles.Any(r => r.RoleId == currentRoleId.Value)
            ? newUserRoles.First(r => r.RoleId == currentRoleId.Value)
            : newUserRoles.First();

        roleToSetCurrent.SetCurrent(true);

        _roles.Clear();
        _roles.AddRange(newUserRoles);

        if (added.Count > 0 || removed.Count > 0)
        {
            AddDomainEvent(new UserRolesChangedDomainEvent(Id, added, removed));
        }
    }

    /// <summary>
    /// 切换当前使用角色。
    /// </summary>
    /// <param name="roleId">要切换到的角色 ID。</param>
    /// <exception cref="UserRoleNotAssignedException">用户未分配指定角色时抛出。</exception>
    public void SwitchCurrentRole(Guid roleId)
    {
        var userRole = _roles.FirstOrDefault(r => r.RoleId == roleId)
            ?? throw new UserRoleNotAssignedException(Id, roleId);

        foreach (var role in _roles)
        {
            role.SetCurrent(role.RoleId == roleId);
        }
    }

    /// <summary>
    /// 修改用户角色名称快照。
    /// </summary>
    /// <param name="roleId">角色 ID。</param>
    /// <param name="roleName">新的角色名称。</param>
    public void ChangeRoleName(Guid roleId, string roleName)
    {
        var target = _roles.FirstOrDefault(r => r.RoleId == roleId);
        target?.Rename(roleName);
    }

    /// <summary>
    /// 修改用户所属部门。
    /// </summary>
    /// <param name="departmentId">部门 ID。</param>
    /// <param name="departmentName">部门名称。</param>
    public void ChangeDepartment(Guid departmentId, string departmentName)
    {
        var department = new UserDepartment(departmentId, departmentName);

        if (Department != department)
        {
            Department = department;
        }
    }

    /// <summary>
    /// 重置用户密码，并登记密码重置领域事件。
    /// </summary>
    /// <param name="newPasswordHash">新密码哈希。</param>
    /// <param name="newPassword">新明文密码。</param>
    /// <remarks>
    /// 明文密码仅用于领域事件后的通知场景，持久化时不应保存明文密码。
    /// </remarks>
    public void ResetPassword(string newPasswordHash, string newPassword)
    {
        PasswordHash = newPasswordHash;

        AddDomainEvent(new UserPasswordResetDomainEvent(Id, Email, newPassword));
    }

    /// <summary>
    /// 启用用户账号，并登记账号启用领域事件。
    /// </summary>
    /// <exception cref="UserAlreadyEnabledException">用户已启用时抛出。</exception>
    public void Enable()
    {
        if (Status == UserStatus.Enabled)
        {
            throw new UserAlreadyEnabledException(Id);
        }

        Status = UserStatus.Enabled;

        AddDomainEvent(new UserEnabledDomainEvent(Id, Email));
    }

    /// <summary>
    /// 禁用用户账号，并登记账号禁用领域事件。
    /// </summary>
    /// <exception cref="UserAlreadyDisabledException">用户已禁用时抛出。</exception>
    public void Disable()
    {
        if (Status == UserStatus.Disabled)
        {
            throw new UserAlreadyDisabledException(Id);
        }

        Status = UserStatus.Disabled;

        AddDomainEvent(new UserDisabledDomainEvent(Id, Email));
    }
}
