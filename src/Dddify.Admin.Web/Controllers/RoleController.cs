using Dddify.Admin.Application.Commands.Roles;
using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Queries.Permissions;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Web.Requests.Roles;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 角色管理。
/// </summary>
/// <param name="sender">请求发送器。</param>
[Route("api/v1/roles")]
public class RoleController(ISender sender) : BaseController
{
    /// <summary>
    /// 获取所有角色。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("all", Name = "GetAllRoles")]
    [ProducesResponseType<ApiResult<IEnumerable<RoleListDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<RoleListDto>> GetAllRolesAsync(CancellationToken cancellationToken)
    {
        return await sender.Send(new GetAllRolesQuery(), cancellationToken);
    }

    /// <summary>
    /// 查询角色列表。
    /// </summary>
    /// <param name="current">当前页码。</param>
    /// <param name="pageSize">每页数量。</param>
    /// <param name="name">角色名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "SearchRoles")]
    [Permission("system:role:index")]
    [ProducesResponseType<ApiResult<PagedResult<RoleListDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<IPagedResult<RoleListDto>> SearchAsync(
        [FromQuery] int current,
        [FromQuery] int pageSize,
        [FromQuery] string? name,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new SearchRolesQuery(current, pageSize, name), cancellationToken);
    }

    /// <summary>
    /// 获取角色详情。
    /// </summary>
    /// <param name="id">角色ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "GetRoleDetail")]
    [Permission("system:role:index")]
    [ProducesResponseType<ApiResult<RoleDetailDto>>(StatusCodes.Status200OK)]
    public async Task<RoleDetailDto> GetRoleDetailAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetRoleByIdQuery(id), cancellationToken);
    }

    /// <summary>
    /// 新增角色。
    /// </summary>
    /// <param name="request">新增角色请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost(Name = "CreateRole")]
    [Permission("system:role:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task CreateAsync([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreateRoleCommand(
                request.Name,
                request.IsDefault,
                request.Order,
                request.Description),
            cancellationToken);
    }

    /// <summary>
    /// 修改角色。
    /// </summary>
    /// <param name="id">角色ID。</param>
    /// <param name="request">修改角色请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "UpdateRole")]
    [Permission("system:role:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateRoleCommand(
                id,
                request.Name,
                request.IsDefault,
                request.Order,
                request.Description,
                request.ConcurrencyStamp),
            cancellationToken);
    }

    /// <summary>
    /// 删除角色。
    /// </summary>
    /// <param name="id">角色ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeleteRole")]
    [Permission("system:role:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteRoleCommand(id), cancellationToken);
    }

    /// <summary>
    /// 获取角色权限。
    /// </summary>
    /// <param name="id">角色ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}/permissions", Name = "GetRolePermissions")]
    [ProducesResponseType<ApiResult<IEnumerable<RolePermissionDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<RolePermissionDto>> GetRolePermissionsAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetRolePermissionsQuery(id), cancellationToken);
    }

    /// <summary>
    /// 分配角色权限。
    /// </summary>
    /// <param name="id">角色ID。</param>
    /// <param name="permissionIds">权限ID列表。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/permissions", Name = "AssignRolePermissions")]
    [Permission("system:role:assign-permissions")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task AssignRolePermissionsAsync(
        [FromRoute] Guid id,
        [FromBody] IEnumerable<Guid> permissionIds,
        CancellationToken cancellationToken)
    {
        var permissions = await sender.Send(new GetPermissionsByIdsQuery(permissionIds), cancellationToken);
        var permissionEntries = permissions.Select(c => new PermissionEntry(c.Id, c.Code));

        await sender.Send(new AssignPermissionsCommand(id, permissionEntries), cancellationToken);
    }
}
