namespace Dddify.Admin.Application.Caching;

public static class CacheKeys
{
    public static string UserRoles(Guid userId) => $"user:{userId}:roles";

    public static string RolePermissions(Guid roleId) => $"role:{roleId}:permissions";

    public static string LookupActiveItems(string code) => $"lookup:{code}:items";
}
