using Dddify.Admin.Application.Commands.Permissions;
using Dddify.Admin.Application.Dtos.Permissions;
using Dddify.Admin.Application.Queries.Permissions;
using Dddify.Admin.Web.Requests.Permissions;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 权限管理。
/// </summary>
/// <param name="sender">请求发送器。</param>
[Route("api/v1/permissions")]
public class PermissionController(ISender sender) : BaseController
{
    /// <summary>
    /// 获取所有权限。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("all", Name = "GetAllPermissions")]
    [ProducesResponseType<ApiResult<IEnumerable<PermissionDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync(CancellationToken cancellationToken)
    {
        return await sender.Send(new GetAllPermissionsQuery(), cancellationToken);
    }

    /// <summary>
    /// 查询权限列表。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="code">权限标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "SearchPermissions")]
    [Permission("system:permission:index")]
    [ProducesResponseType<ApiResult<IEnumerable<PermissionDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(
        [FromQuery] string? name,
        [FromQuery] string? code,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new SearchPermissionsQuery(name, code), cancellationToken);
    }

    /// <summary>
    /// 新增权限。
    /// </summary>
    /// <param name="request">新增权限请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost(Name = "CreatePermission")]
    [Permission("system:permission:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task CreateAsync([FromBody] CreateOrUpdatePermissionRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreatePermissionCommand(
                request.ParentId,
                request.Code,
                request.Name,
                request.Type,
                request.Order),
            cancellationToken);
    }

    /// <summary>
    /// 修改权限。
    /// </summary>
    /// <param name="id">权限ID。</param>
    /// <param name="request">修改权限请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "UpdatePermission")]
    [Permission("system:permission:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] CreateOrUpdatePermissionRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdatePermissionCommand(
                id,
                request.ParentId,
                request.Code,
                request.Name,
                request.Type,
                request.Order),
            cancellationToken);
    }

    /// <summary>
    /// 删除权限。
    /// </summary>
    /// <param name="id">权限ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeletePermission")]
    [Permission("system:permission:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeletePermissionCommand(id), cancellationToken);
    }
}
