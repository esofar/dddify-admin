using Dddify.Admin.Application.Commands.InboxItems;
using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Dtos.InboxItems;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.InboxItems;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Web.Requests.Auths;
using Dddify.Admin.Web.Requests.InboxItems;

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
    /// 获取当前用户资料。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>当前用户资料、角色及权限。</returns>
    [HttpGet(Name = "GetProfile")]
    [ProducesResponseType<ApiResult<CurrentUserDto>>(StatusCodes.Status200OK)]
    public async Task<CurrentUserDto> GetProfileAsync(CancellationToken cancellationToken)
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
    /// 切换当前角色。
    /// </summary>
    /// <param name="request">切换当前角色请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [HttpPut("roles", Name = "SwitchRole")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task SwitchRoleAsync(
        [FromBody] SwitchUserCurrentRoleRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new SwitchUserRoleCommand(
                currentUser.GetIdAsGuid(),
                request.RoleId),
            cancellationToken);
    }

    /// <summary>
    /// 查询收件箱列表。
    /// </summary>
    /// <param name="request">查询收件箱列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>收件箱消息分页列表。</returns>
    [HttpGet("inbox-items", Name = "SearchMeInboxItems")]
    [ProducesResponseType<ApiResult<PagedResult<InboxItemListDto>>>(StatusCodes.Status200OK)]
    public async Task<IPagedResult<InboxItemListDto>> SearchInboxItemsAsync(
        [FromQuery] SearchInboxItemsRequest request,
        CancellationToken cancellationToken)
    {
        return await sender.Send(
            new SearchInboxItemsQuery(
                currentUser.GetIdAsGuid(),
                request.Current,
                request.PageSize,
                request.IsRead),
            cancellationToken);
    }

    /// <summary>
    /// 获取未读消息数量。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>未读消息数量。</returns>
    [HttpGet("inbox-items/unread-count", Name = "GetUnreadInboxItemCount")]
    [ProducesResponseType<ApiResult<UnreadInboxItemCountDto>>(StatusCodes.Status200OK)]
    public async Task<UnreadInboxItemCountDto> GetUnreadInboxItemCountAsync(CancellationToken cancellationToken)
    {
        return await sender.Send(
            new GetUnreadInboxItemCountQuery(currentUser.GetIdAsGuid()),
            cancellationToken);
    }

    /// <summary>
    /// 标记收件箱消息为已读。
    /// </summary>
    /// <param name="request">标记消息已读请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [HttpPut("inbox-items/read", Name = "MarkInboxItemsAsRead")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task MarkInboxItemsAsReadAsync(
        [FromBody] MarkInboxItemsAsReadRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new MarkInboxItemsAsReadCommand(request.Ids, currentUser.GetIdAsGuid()),
            cancellationToken);
    }

    /// <summary>
    /// 删除收件箱消息。
    /// </summary>
    /// <param name="id">收件箱消息 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    [HttpDelete("inbox-items/{id}", Name = "DeleteMeInboxItem")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task DeleteInboxItemAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteInboxItemCommand(id, currentUser.GetIdAsGuid()),
            cancellationToken);
    }

    /// <summary>
    /// 获取消息来源详情。
    /// </summary>
    /// <param name="id">收件箱消息 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>消息来源详情。</returns>
    [HttpGet("inbox-items/{id}/source", Name = "GetInboxItemSource")]
    [ProducesResponseType<ApiResult<InboxItemSourceDetailDto>>(StatusCodes.Status200OK)]
    public async Task<InboxItemSourceDetailDto> GetInboxItemSourceAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(
            new GetInboxItemSourceQuery(id, currentUser.GetIdAsGuid()),
            cancellationToken);
    }
}
