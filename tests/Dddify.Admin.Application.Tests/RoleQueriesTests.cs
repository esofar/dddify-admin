using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Domain.Aggregates.Roles;
using Dddify.Admin.Domain.Aggregates.Users;
using Dddify.Admin.Domain.Repositories;
using MapsterMapper;
using System.Reflection;

namespace Dddify.Admin.Application.Tests;

public class RoleQueriesTests
{
    [Fact]
    public async Task User_Roles_Are_Read_From_Repository_With_Request_Cancellation()
    {
        var user = CreateUser();
        var roleId = Guid.NewGuid();
        user.AssignRoles([new UserRole(user.Id, roleId, "Current role")]);
        using var cancellation = new CancellationTokenSource();
        var repository = Stub<IUserRepository>((method, args) =>
        {
            Assert.Equal("GetUserWithRolesAsync", method);
            Assert.Equal(user.Id, args![0]);
            Assert.Equal(cancellation.Token, args[1]);
            return Task.FromResult<User?>(user);
        });

        var roles = await new GetUserRolesQueryHandler(repository, new Mapper())
            .Handle(new GetUserRolesQuery(user.Id), cancellation.Token);

        Assert.Equal(roleId, Assert.Single(roles).RoleId);
    }

    [Fact]
    public async Task Role_Switch_Is_Visible_On_The_Next_Query()
    {
        var user = CreateUser();
        var firstRoleId = Guid.NewGuid();
        var secondRoleId = Guid.NewGuid();
        user.AssignRoles([new UserRole(user.Id, firstRoleId, "First"), new UserRole(user.Id, secondRoleId, "Second")]);
        var reads = 0;
        var repository = Stub<IUserRepository>((_, _) =>
        {
            reads++;
            return Task.FromResult<User?>(user);
        });
        var handler = new GetUserRolesQueryHandler(repository, new Mapper());

        await handler.Handle(new GetUserRolesQuery(user.Id), default);
        user.SwitchCurrentRole(secondRoleId);
        var roles = await handler.Handle(new GetUserRolesQuery(user.Id), default);

        Assert.Equal(secondRoleId, Assert.Single(roles, role => role.IsCurrent).RoleId);
        Assert.Equal(2, reads);
    }

    [Fact]
    public async Task Role_Permissions_Are_Read_From_Repository_With_Request_Cancellation()
    {
        var role = new Role(Guid.NewGuid(), "Current role", false, 0, null);
        role.AssignPermissions([new RolePermission(role.Id, Guid.NewGuid(), "current:permission")]);
        using var cancellation = new CancellationTokenSource();
        var repository = Stub<IRoleRepository>((method, args) =>
        {
            Assert.Equal("GetRoleWithPermissionsAsync", method);
            Assert.Equal(role.Id, args![0]);
            Assert.Equal(cancellation.Token, args[1]);
            return Task.FromResult<Role?>(role);
        });

        var permissions = await new GetRolePermissionsQueryHandler(repository)
            .Handle(new GetRolePermissionsQuery(role.Id), cancellation.Token);

        Assert.Equal("current:permission", Assert.Single(permissions).PermissionCode);
    }

    [Fact]
    public async Task Permission_Changes_Are_Visible_On_The_Next_Query()
    {
        var role = new Role(Guid.NewGuid(), "Role", false, 0, null);
        role.AssignPermissions([new RolePermission(role.Id, Guid.NewGuid(), "old:permission")]);
        var reads = 0;
        var repository = Stub<IRoleRepository>((_, _) =>
        {
            reads++;
            return Task.FromResult<Role?>(role);
        });
        var handler = new GetRolePermissionsQueryHandler(repository);

        await handler.Handle(new GetRolePermissionsQuery(role.Id), default);
        role.AssignPermissions([new RolePermission(role.Id, Guid.NewGuid(), "new:permission")]);
        var permissions = await handler.Handle(new GetRolePermissionsQuery(role.Id), default);

        Assert.Equal("new:permission", Assert.Single(permissions).PermissionCode);
        Assert.Equal(2, reads);
    }

    [Fact]
    public async Task Role_Rename_Updates_All_Affected_Users()
    {
        var roleId = Guid.NewGuid();
        var users = new[] { CreateUser(), CreateUser() };
        foreach (var user in users) user.AssignRoles([new UserRole(user.Id, roleId, "Old name")]);
        var repository = Stub<IUserRepository>((method, args) =>
        {
            Assert.Equal("GetUsersByRoleIdAsync", method);
            Assert.Equal(roleId, args![0]);
            return Task.FromResult<IEnumerable<User>>(users);
        });

        await new RefreshUsersRoleCommandHandler(repository).Handle(new RefreshUsersRoleCommand(roleId, "New name"), default);

        Assert.All(users, user => Assert.Equal("New name", Assert.Single(user.Roles).RoleName));
    }

    [Fact]
    public async Task Assigning_And_Switching_Roles_Updates_Current_Role()
    {
        var user = CreateUser();
        var firstRoleId = Guid.NewGuid();
        var secondRoleId = Guid.NewGuid();
        var repository = Stub<IUserRepository>((_, _) => Task.FromResult<User?>(user));

        await new AssignUserRolesCommandHandler(repository).Handle(
            new AssignUserRolesCommand(user.Id, [new(firstRoleId, "First"), new(secondRoleId, "Second")]), default);
        await new SwitchUserRoleCommandHandler(repository).Handle(new SwitchUserRoleCommand(user.Id, secondRoleId), default);

        Assert.Equal(secondRoleId, Assert.Single(user.Roles, role => role.IsCurrent).RoleId);
    }

    private static User CreateUser() => new(Guid.NewGuid(), "hash", "Member", null, UserGender.Male,
        null, "member@example.com", "13800138000", Guid.NewGuid(), "Department");

    private static T Stub<T>(Func<string, object?[]?, object?> callback) where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).Callback = callback;
        return proxy;
    }

    public class TestProxy : DispatchProxy
    {
        public Func<string, object?[]?, object?> Callback { get; set; } = default!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Callback(method!.Name, args);
    }
}
