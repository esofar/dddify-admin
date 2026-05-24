namespace Dddify.Admin.Application.Services.Caching;

public static class CacheKeys
{
    public static class User
    {
        public static string Roles(Guid userId) => $"user:{userId}:roles";
    }

    public static class Role
    {
        public static string Permissions(Guid roleId) => $"role:{roleId}:permissions";
    }

    public static class Lookup
    {
        public static string Items(string code) => $"lookup:{code}:items";
    }
}
