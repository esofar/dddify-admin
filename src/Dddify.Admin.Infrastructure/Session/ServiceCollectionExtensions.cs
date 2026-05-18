namespace Dddify.Admin.Infrastructure.Session;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSessionCleanup(this IServiceCollection services)
    {
        services
            .AddOptions<SessionCleanupOptions>()
            .BindConfiguration(SessionCleanupOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<SessionCleanupHostedService>();

        return services;
    }
}