using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.Dictionaries;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Host.Models.Users;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/users")]
public class UserController : BaseController
{
    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("current")]
    public async Task<UserProfileDto> GetCurrentUserAsync()
    {
        return await SendAsync(new GetUserProfileQuery());
    }

    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="current">当前页</param>
    /// <param name="pageSize">页容量</param>
    /// <param name="fullName">姓名</param>
    /// <param name="gender">性别</param>
    /// <param name="organizationId">机构ID</param>
    /// <param name="status">账号状态</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(IApiResult<IPagedResult<UserDto>>))]
    [HttpGet]
    public async Task<IPagedResult<UserDto>> GetAsync(int current, int pageSize, string? fullName, UserGender? gender, Guid? organizationId, UserStatus? status)
    {
        return await SendAsync(new GetUsersQuery(
            current,
            pageSize,
            fullName,
            gender,
            organizationId,
            status));
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<UserDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetUserByIdQuery(id));
    }

    /// <summary>
    /// 新建用户
    /// </summary>
    /// <param name="request">新增模型</param>
    /// <returns></returns>
    [HttpPost]
    public async Task CreateAsync([FromBody] CreateUserRequest request)
    {
        await SendAsync(new CreateUserCommand(
            request.FullName,
            request.NickName,
            request.Gender,
            request.BirthDate,
            request.Email,
            request.PhoneNumber,
            request.OrganizationId,
            request.Type));
    }

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">更新模型</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, [FromBody] UpdateUserRequest request)
    {
        await SendAsync(new UpdateUserCommand(
            id,
            request.FullName,
            request.NickName,
            request.Gender,
            request.BirthDate,
            request.Email,
            request.PhoneNumber,
            request.OrganizationId,
            request.Type,
            request.ConcurrencyStamp));
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteUserCommand(id));
    }

    /// <summary>
    /// 人员类型列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("types")]
    public async Task<IEnumerable<DictionaryItemDto>> GetTypesAsync()
    {
        return await SendAsync(new GetItemsByDictionaryCodeQuery(DictionaryCodes.UserType));
    }
}