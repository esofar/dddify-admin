namespace Dddify.Admin.Host.Models.Users;

public class CreateUserRequest
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// 机构ID
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// 人员类型
    /// </summary>
    public string Type { get; set; }
}