using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Web.Requests.Auths;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 当前用户。
/// </summary>
/// <param name="sender">请求发送器。</param>
/// <param name="currentUser">当前登录用户。</param>
[Route("api/v1/me")]
public class MeController(ISender sender, ICurrentUser currentUser) : BaseController
{
    /// <summary>
    /// 获取当前用户信息。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "GetMe")]
    [ProducesResponseType<ApiResult<CurrentUserDto>>(StatusCodes.Status200OK)]
    public async Task<CurrentUserDto> GetAsync(CancellationToken cancellationToken)
    {
        var user = await sender.Send(new GetUserByIdQuery(currentUser.GetIdAsGuid()), cancellationToken);

        var userRoles = await sender.Send(new GetUserRolesQuery(user.Id), cancellationToken);

        var currentRoleId = userRoles.First(c => c.IsCurrent).RoleId;

        var rolePermissions = await sender.Send(new GetRolePermissionsQuery(currentRoleId), cancellationToken);

        return new CurrentUserDto(
            user.Id,
            user.Name,
            user.NickName,
            user.Avatar,
            user.Email,
            user.PhoneNumber,
            currentRoleId,
            userRoles,
            rolePermissions.Select(c => c.PermissionCode));
    }

    /// <summary>
    /// 切换当前用户角色。
    /// </summary>
    /// <param name="request">切换当前角色请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("roles", Name = "SwitchRole")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task SwitchRoleAsync([FromBody] SwitchUserCurrentRoleRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new SwitchUserRoleCommand(currentUser.GetIdAsGuid(), request.RoleId), cancellationToken);
    }
}
