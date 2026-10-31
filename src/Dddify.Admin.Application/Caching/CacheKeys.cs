namespace Dddify.Admin.Application.Caching;

public static class CacheKeys
{
    public static string LookupActiveItems(string code) => $"lookup:{code}:items";
}
