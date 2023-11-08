namespace Dddify.Admin.Host.Models.Users;

public class UpdateUserRequest: CreateUserRequest
{
    /// <summary>
    /// 并发标识
    /// </summary>
    public string ConcurrencyStamp { get; set; }
}