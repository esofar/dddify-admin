using Dddify.Admin.Application.Abstractions.Email;
using Dddify.Admin.Application.Configuration.Email;
using MimeKit;

namespace Dddify.Admin.Infrastructure.Email;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailVerificationCode(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddOptions<EmailVerificationCodeOptions>()
            .Bind(configuration.GetSection(EmailVerificationCodeOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .Validate(settings => !settings.UseDevelopmentSender || environment.IsDevelopment(),
                "The development email sender can only be enabled in Development.")
            .Validate(settings => settings.UseDevelopmentSender ||
                (!string.IsNullOrWhiteSpace(settings.Host) && settings.Port is > 0 and <= 65535 &&
                 MailboxAddress.TryParse(settings.From, out _)),
                "Configure Email:Host, Email:Port and Email:From for SMTP delivery.")
            .ValidateOnStart();

        services.AddScoped<IEmailVerificationCodeService, RedisEmailVerificationCodeService>();
        services.AddScoped<IEmailSender>(provider =>
            provider.GetRequiredService<IOptions<EmailOptions>>().Value.UseDevelopmentSender
                ? ActivatorUtilities.CreateInstance<FakeEmailSender>(provider)
                : ActivatorUtilities.CreateInstance<SmtpEmailSender>(provider));
        return services;
    }
}
