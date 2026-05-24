using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.Departments;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Web.Requests.Users;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 用户管理。
/// </summary>
/// <param name="sender">请求发送器。</param>
[Route("api/v1/users")]
public class UserController(ISender sender) : BaseController
{
    /// <summary>
    /// 查询用户列表。
    /// </summary>
    /// <param name="request">查询用户列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "SearchUsers")]
    [Permission("system:user:index")]
    [ProducesResponseType<ApiResult<PagedResult<UserListDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<IPagedResult<UserListDto>> SearchAsync([FromQuery] SearchUserRequest request, CancellationToken cancellationToken)
    {
        return await sender.Send(
            new SearchUsersQuery(
                request.Current,
                request.PageSize,
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.DepartmentId,
                request.RoleId,
                request.Gender,
                request.Status),
            cancellationToken);
    }

    /// <summary>
    /// 获取用户详情。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "GetUserDetail")]
    [ProducesResponseType<ApiResult<UserDetailDto>>(StatusCodes.Status200OK)]
    public async Task<UserDetailDto> GetUserDetailAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetUserByIdQuery(id), cancellationToken);
    }

    /// <summary>
    /// 新增用户。
    /// </summary>
    /// <param name="request">新增用户请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost(Name = "CreateUser")]
    [Permission("system:user:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task CreateAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var department = await sender.Send(new GetDepartmentByIdQuery(request.DepartmentId), cancellationToken);
        var defaultRoles = await sender.Send(new GetDefaultRolesQuery(), cancellationToken);

        var userRoles = defaultRoles.Select(c => new RoleEntry(c.Id, c.Name));

        await sender.Send(
            new CreateUserCommand(
                request.Password,
                request.Name,
                request.NickName,
                request.Gender,
                request.BirthDate,
                request.Email,
                request.PhoneNumber,
                department.Id,
                department.Name,
                userRoles),
            cancellationToken);
    }

    /// <summary>
    /// 修改用户。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="request">修改用户请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "UpdateUser")]
    [Permission("system:user:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var department = await sender.Send(new GetDepartmentByIdQuery(request.DepartmentId), cancellationToken);

        await sender.Send(
            new UpdateUserCommand(
                id,
                request.Name,
                request.NickName,
                request.Gender,
                request.BirthDate,
                request.Email,
                request.PhoneNumber,
                department.Id,
                department.Name),
            cancellationToken);
    }

    /// <summary>
    /// 删除用户。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeleteUser")]
    [Permission("system:user:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteUserCommand(id), cancellationToken);
    }

    /// <summary>
    /// 获取用户角色。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}/roles", Name = "GetUserRoles")]
    [ProducesResponseType<ApiResult<IEnumerable<UserRoleDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<UserRoleDto>> GetUserRolesAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetUserRolesQuery(id), cancellationToken);
    }

    /// <summary>
    /// 分配用户角色。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="roleIds">角色ID列表。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/roles", Name = "AssignUserRoles")]
    [Permission("system:user:assign-roles")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task AssignUserRolesAsync(
        [FromRoute] Guid id,
        [FromBody] IEnumerable<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        var roles = await sender.Send(new GetRolesByIdsQuery(roleIds), cancellationToken);
        var roleEntries = roles.Select(c => new RoleEntry(c.Id, c.Name));

        await sender.Send(new AssignUserRolesCommand(id, roleEntries), cancellationToken);
    }

    /// <summary>
    /// 重置用户密码。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPatch("{id}/password", Name = "ResetUserPassword")]
    [Permission("system:user:reset-password")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task ResetUserPasswordAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ResetUserPasswordCommand(id), cancellationToken);
    }

    /// <summary>
    /// 启用用户。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPatch("{id}/enable", Name = "EnableUser")]
    [Permission("system:user:enable")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task EnableUserAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new EnableUserCommand(id), cancellationToken);
    }

    /// <summary>
    /// 禁用用户。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPatch("{id}/disable", Name = "DisableUser")]
    [Permission("system:user:disable")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DisableUserAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DisableUserCommand(id), cancellationToken);
    }
}
