namespace Dddify.Admin.Infrastructure.Locking;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedLock(this IServiceCollection services)
    {
        services.AddSingleton<IDistributedLock, RedisDistributedLock>();

        return services;
    }
}
