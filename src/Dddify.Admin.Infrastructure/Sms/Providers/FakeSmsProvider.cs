namespace Dddify.Admin.Infrastructure.Sms.Providers;

public sealed class FakeSmsProvider(
    IOptions<SmsOptions> options,
    ILogger<FakeSmsProvider> logger) : ISmsProvider
{
    public string Name => SmsProviderNames.Fake;

    public async Task<SmsProviderResult> SendAsync(SmsProviderRequest request, CancellationToken cancellationToken)
    {
        if (options.Value.Fake.LatencyMilliseconds > 0)
        {
            await Task.Delay(options.Value.Fake.LatencyMilliseconds, cancellationToken);
        }

        if (options.Value.Fake.LogMessageContent && logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Fake sms sent. Scene={Scene}, Phone={Phone}, Template={TemplateCode}, Parameters={Parameters}, TraceId={TraceId}",
                request.Scene,
                PhoneNumberProtector.Mask(request.PhoneNumber),
                request.TemplateCode,
                string.Join(",", request.Parameters.Select(p => $"{p.Key}={p.Value}")),
                request.TraceId);
        }

        return SmsProviderResult.Success($"fake-{request.RequestId}");
    }
}
