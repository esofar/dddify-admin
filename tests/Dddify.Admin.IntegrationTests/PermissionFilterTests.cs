using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Web.Authorization;
using Dddify.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace Dddify.Admin.IntegrationTests;

public class PermissionFilterTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Authorization_Uses_Uncached_Roles_And_Permissions(bool hasPermission)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var roleQueries = 0;
        var permissionQueries = 0;
        using var cancellation = new CancellationTokenSource();
        var context = CreateContext(cancellation.Token);
        var sender = Stub<ISender>((method, args) =>
        {
            Assert.Equal("Send", method.Name);
            Assert.Equal(cancellation.Token, args![1]);
            switch (args[0])
            {
                case GetUserRolesQuery query:
                    roleQueries++;
                    Assert.Equal(userId, query.UserId);
                    return Task.FromResult<IEnumerable<UserRoleDto>>([new(roleId, "Current role", true)]);
                case GetRolePermissionsQuery query:
                    permissionQueries++;
                    Assert.Equal(roleId, query.RoleId);
                    return Task.FromResult<IEnumerable<RolePermissionDto>>(hasPermission
                        ? [new(Guid.NewGuid().ToString(), "required:permission")]
                        : []);
                default:
                    throw new InvalidOperationException("Unexpected query.");
            }
        });
        var filter = new CheckUserPermissionFilter(CurrentUser(userId), sender,
            NullLogger<CheckUserPermissionFilter>.Instance);

        await filter.OnAuthorizationAsync(context);

        if (hasPermission) Assert.Null(context.Result);
        else Assert.IsType<ForbidResult>(context.Result);
        Assert.Equal(1, roleQueries);
        Assert.Equal(1, permissionQueries);
    }

    [Fact]
    public async Task Removed_User_Roles_Are_Denied_Without_Querying_Permissions()
    {
        var sender = Stub<ISender>((_, args) =>
        {
            var query = Assert.IsType<GetUserRolesQuery>(args![0]);
            return Task.FromResult<IEnumerable<UserRoleDto>>([]);
        });
        var context = CreateContext(default);
        var filter = new CheckUserPermissionFilter(CurrentUser(Guid.NewGuid()), sender,
            NullLogger<CheckUserPermissionFilter>.Instance);

        await filter.OnAuthorizationAsync(context);

        Assert.IsType<ForbidResult>(context.Result);
    }

    private static AuthorizationFilterContext CreateContext(CancellationToken cancellationToken)
    {
        var httpContext = new DefaultHttpContext { RequestAborted = cancellationToken };
        httpContext.SetEndpoint(new Endpoint(_ => Task.CompletedTask,
            new EndpointMetadataCollection(
                new PermissionAttribute("other:permission"),
                new PermissionAttribute("required:permission")), "test"));
        return new AuthorizationFilterContext(
            new ActionContext(httpContext, new RouteData(), new ActionDescriptor()), []);
    }

    private static ICurrentUser CurrentUser(Guid userId) => Stub<ICurrentUser>((method, _) => method.Name switch
    {
        "get_IsAuthenticated" => true,
        "get_Id" => userId.ToString(),
        _ => throw new InvalidOperationException(method.Name)
    });

    private static T Stub<T>(Func<MethodInfo, object?[]?, object?> callback) where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).Callback = callback;
        return proxy;
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Callback { get; set; } = default!;

        protected override object? Invoke(MethodInfo? method, object?[]? args) => Callback(method!, args);
    }
}
