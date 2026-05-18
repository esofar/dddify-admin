using Dddify.Admin.Infrastructure.Sms.Providers;

namespace Dddify.Admin.Infrastructure.Sms;

public static class SmsExtensions
{
    public static IServiceCollection AddSms(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var smsSection = configuration.GetSection(SmsOptions.SectionName);
        var isProduction = environment.IsProduction();

        services
            .AddOptions<SmsOptions>()
            .Bind(smsSection)
            .ValidateDataAnnotations()
            .Validate(options => options.Templates.Count > 0, "At least one sms template must be configured.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Verification.HashSecret), "Sms:Verification:HashSecret cannot be empty.")
            .Validate(options => !isProduction ||
                (!options.DefaultProvider.Equals(SmsProviderNames.Fake, StringComparison.OrdinalIgnoreCase) &&
                 !options.FallbackProviders.Any(provider => provider.Equals(SmsProviderNames.Fake, StringComparison.OrdinalIgnoreCase))),
                "Fake sms provider cannot be enabled in Production.")
            .ValidateOnStart();

        services.AddScoped<ISmsSender, SmsSender>();
        services.AddScoped<ISmsVerificationCodeService, RedisSmsVerificationCodeService>();
        services.AddScoped<ISmsRateLimiter, RedisSmsRateLimiter>();

        services.AddScoped<ISmsProvider, FakeSmsProvider>();
        services.AddScoped<ISmsProvider, AliyunSmsProvider>();
        services.AddScoped<ISmsProvider, TencentSmsProvider>();

        return services;
    }
}
