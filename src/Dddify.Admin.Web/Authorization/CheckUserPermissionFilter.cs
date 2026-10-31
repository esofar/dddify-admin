using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;

namespace Dddify.Admin.Web.Authorization;

/// <summary>
/// 按当前角色校验接口访问权限。
/// </summary>
/// <param name="currentUser">当前登录用户。</param>
/// <param name="sender">请求发送器。</param>
/// <param name="logger">日志记录器。</param>
/// <remarks>声明多个权限时，满足任意一个即可。</remarks>
public class CheckUserPermissionFilter(ICurrentUser currentUser, ISender sender, ILogger<CheckUserPermissionFilter> logger) : IAsyncAuthorizationFilter
{
    /// <summary>
    /// 校验当前用户的访问权限。
    /// </summary>
    /// <param name="context">授权上下文。</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        var permissionAttrs = endpoint?.Metadata.GetOrderedMetadata<PermissionAttribute>();

        if (permissionAttrs is null || !permissionAttrs.Any())
        {
            return;
        }

        if (!currentUser.IsAuthenticated)
        {
            logger.LogWarning("Permission denied. User is not authenticated.");
            context.Result = new ForbidResult();
            return;
        }

        var permissionCodes = permissionAttrs.Select(p => p.Code).Distinct().ToList();

        // 授权查询直接读库，及时反映角色与权限变更。
        var cancellationToken = context.HttpContext.RequestAborted;
        var userRoles = await sender.Send(
            new GetUserRolesQuery(currentUser.GetIdAsGuid()), cancellationToken);

        if (!userRoles.Any())
        {
            logger.LogWarning("Permission denied. User {UserId} does not have a current role assigned.", currentUser.Id);
            context.Result = new ForbidResult();
            return;
        }

        var currentUserRole = userRoles.First(c => c.IsCurrent);

        var rolePermissions = await sender.Send(
            new GetRolePermissionsQuery(currentUserRole.RoleId), cancellationToken);
        var permissionCodeHashSet = rolePermissions.Select(p => p.PermissionCode).ToHashSet();

        foreach (var permissionCode in permissionCodes)
        {
            if (permissionCodeHashSet.Contains(permissionCode))
            {
                return;
            }
        }

        logger.LogWarning("Permission denied. User {UserId} lacks one of required: {PermissionCodes}", currentUser.Id, string.Join(", ", permissionCodes));
        context.Result = new ForbidResult();
    }
}
