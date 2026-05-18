using StackExchange.Redis;

namespace Dddify.Admin.Infrastructure.Caching;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSection = configuration.GetSection(RedisOptions.SectionName);
        var redisOptions = redisSection.Get<RedisOptions>()
            ?? throw new InvalidOperationException($"Missing '{RedisOptions.SectionName}' configuration section.");

        if (string.IsNullOrWhiteSpace(redisOptions.Configuration))
        {
            throw new InvalidOperationException($"'{RedisOptions.SectionName}:Configuration' cannot be empty.");
        }

        services.Configure<RedisOptions>(redisSection);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var connectionOptions = ConfigurationOptions.Parse(redisOptions.Configuration);

            connectionOptions.AbortOnConnectFail = false;
            connectionOptions.ConnectRetry = 3;
            connectionOptions.ConnectTimeout = 5_000;
            connectionOptions.SyncTimeout = 5_000;
            connectionOptions.AsyncTimeout = 5_000;

            return ConnectionMultiplexer.Connect(connectionOptions);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisOptions.Configuration;
            options.InstanceName = redisOptions.InstanceName;
        });

        return services;
    }
}
