using Dddify.Admin.Application.Caching;
using Dddify.Admin.Application.Commands.Users;
using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Dtos.Users;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Domain.Aggregates.Roles;
using Dddify.Admin.Domain.Aggregates.Users;
using Dddify.Admin.Domain.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Dddify.Admin.Application.Tests;

public class RoleCachingTests
{
    [Fact]
    public async Task Authorization_UserRoles_Bypasses_Stale_Cache_And_Does_Not_Write_Cache()
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
        var handler = new GetUserRolesQueryHandler(repository, UnusedCache(), new Mapper());

        var roles = await handler.Handle(new GetUserRolesQuery(user.Id, UseCache: false), cancellation.Token);

        Assert.Equal(roleId, Assert.Single(roles).RoleId);
    }

    [Fact]
    public async Task Display_UserRoles_Still_Uses_Cache()
    {
        var userId = Guid.NewGuid();
        var cached = new[] { new UserRoleDto(Guid.NewGuid(), "Cached role", true) };
        var cache = CachedValue(CacheKeys.UserRoles(userId), cached);
        var repository = Stub<IUserRepository>((method, _) => throw new InvalidOperationException(method));
        var handler = new GetUserRolesQueryHandler(repository, cache, new Mapper());

        var roles = await handler.Handle(new GetUserRolesQuery(userId), default);

        Assert.Equal(cached, roles);
    }

    [Fact]
    public async Task Authorization_RolePermissions_Bypasses_Stale_Cache_And_Does_Not_Write_Cache()
    {
        var role = new Role(Guid.NewGuid(), "Current role", false, 0, null);
        role.AssignPermissions([new RolePermission(role.Id, Guid.NewGuid(), "current:permission")]);
        var repository = Stub<IRoleRepository>((method, _) =>
        {
            Assert.Equal("GetRoleWithPermissionsAsync", method);
            return Task.FromResult<Role?>(role);
        });
        var handler = new GetRolePermissionsQueryHandler(repository, UnusedCache());

        var permissions = await handler.Handle(new GetRolePermissionsQuery(role.Id, UseCache: false), default);

        Assert.Equal("current:permission", Assert.Single(permissions).PermissionCode);
    }

    [Fact]
    public async Task Display_RolePermissions_Still_Uses_Cache()
    {
        var roleId = Guid.NewGuid();
        var cached = new[] { new RolePermissionDto(Guid.NewGuid().ToString(), "cached:permission") };
        var cache = CachedValue(CacheKeys.RolePermissions(roleId), cached);
        var repository = Stub<IRoleRepository>((method, _) => throw new InvalidOperationException(method));
        var handler = new GetRolePermissionsQueryHandler(repository, cache);

        var permissions = await handler.Handle(new GetRolePermissionsQuery(roleId), default);

        Assert.Equal(cached, permissions);
    }

    [Fact]
    public async Task Role_Rename_Queues_All_Affected_User_Caches()
    {
        var roleId = Guid.NewGuid();
        var users = new[] { CreateUser(), CreateUser() };
        foreach (var user in users)
        {
            user.AssignRoles([new UserRole(user.Id, roleId, "Old name")]);
        }

        var repository = Stub<IUserRepository>((method, _) =>
        {
            Assert.Equal("GetUsersByRoleIdAsync", method);
            return Task.FromResult<IEnumerable<User>>(users);
        });
        var queue = new CacheInvalidationQueue();
        var handler = new RefreshUsersRoleCommandHandler(repository, queue);

        await handler.Handle(new RefreshUsersRoleCommand(roleId, "New name"), default);

        Assert.All(users, user => Assert.Equal("New name", Assert.Single(user.Roles).RoleName));
        Assert.Equal(users.Select(user => CacheKeys.UserRoles(user.Id)).Order(), queue.Drain().Order());
    }

    [Fact]
    public async Task Assigning_And_Switching_Roles_Queues_One_Invalidation_Per_User()
    {
        var user = CreateUser();
        var firstRoleId = Guid.NewGuid();
        var secondRoleId = Guid.NewGuid();
        var repository = Stub<IUserRepository>((_, _) => Task.FromResult<User?>(user));
        var queue = new CacheInvalidationQueue();

        await new AssignUserRolesCommandHandler(repository, queue).Handle(
            new AssignUserRolesCommand(user.Id, [new(firstRoleId, "First"), new(secondRoleId, "Second")]), default);
        await new SwitchUserRoleCommandHandler(repository, queue).Handle(
            new SwitchUserRoleCommand(user.Id, secondRoleId), default);

        Assert.Equal(secondRoleId, Assert.Single(user.Roles, role => role.IsCurrent).RoleId);
        Assert.Equal(CacheKeys.UserRoles(user.Id), Assert.Single(queue.Drain()));
    }

    private static User CreateUser() => new(Guid.NewGuid(), "hash", "Member", null, UserGender.Male,
        null, "member@example.com", "13800138000", Guid.NewGuid(), "Department");

    private static IDistributedCache UnusedCache() =>
        Stub<IDistributedCache>((method, _) => throw new InvalidOperationException($"Unexpected cache access: {method}"));

    private static IDistributedCache CachedValue<T>(string key, T value) =>
        Stub<IDistributedCache>((method, args) =>
        {
            Assert.Equal("GetAsync", method);
            Assert.Equal(key, args![0]);
            return Task.FromResult<byte[]?>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)));
        });

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
