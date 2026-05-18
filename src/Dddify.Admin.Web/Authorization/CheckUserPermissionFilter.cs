using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;

namespace Dddify.Admin.Web.Authorization;

/// <summary>
/// 根据接口上的 <see cref="PermissionAttribute"/> 校验当前用户是否拥有访问权限。
/// </summary>
/// <param name="currentUser">当前登录用户上下文。</param>
/// <param name="sender">MediatR 发送器，用于查询用户角色与角色权限。</param>
/// <param name="logger">权限校验日志记录器。</param>
public class CheckUserPermissionFilter(ICurrentUser currentUser, ISender sender, ILogger<CheckUserPermissionFilter> logger) : IAsyncAuthorizationFilter
{
    /// <summary>
    /// 执行接口权限校验。
    /// </summary>
    /// <param name="context">授权过滤器上下文。</param>
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

        var userRoles = await sender.Send(new GetUserRolesQuery(currentUser.GetIdAsGuid()));

        if (!userRoles.Any())
        {
            logger.LogWarning("Permission denied. User {UserId} does not have a current role assigned.", currentUser.Id);
            context.Result = new ForbidResult();
            return;
        }

        var currentUserRole = userRoles.First(c => c.IsCurrent);

        foreach (var permissionCode in permissionCodes)
        {
            var rolePermissions = await sender.Send(new GetRolePermissionsQuery(currentUserRole.RoleId));
            var permissionCodeHashSet = rolePermissions.Select(p => p.PermissionCode).ToHashSet();

            if (permissionCodeHashSet.Contains(permissionCode))
            {
                return;
            }
        }

        logger.LogWarning("Permission denied. User {UserId} lacks one of required: {PermissionCodes}", currentUser.Id, string.Join(", ", permissionCodes));
        context.Result = new ForbidResult();
    }
}
