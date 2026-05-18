using Polly;
using Polly.Timeout;

namespace Dddify.Admin.Infrastructure.Sms;

public sealed class SmsSender(
    IEnumerable<ISmsProvider> providers,
    IOptions<SmsOptions> options,
    ILogger<SmsSender> logger) : ISmsSender
{
    private readonly Dictionary<string, ISmsProvider> providersByName =
        providers.ToDictionary(provider => provider.Name, StringComparer.OrdinalIgnoreCase);

    public async Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken)
    {
        var template = GetTemplate(message.TemplateName);
        var providerNames = GetProviderNames();

        SmsProviderResult? lastResult = null;
        string? lastProvider = null;

        foreach (var providerName in providerNames)
        {
            if (!providersByName.TryGetValue(providerName, out var provider))
            {
                logger.LogWarning("Configured sms provider is not registered. Provider={Provider}", providerName);
                continue;
            }

            var providerRequest = CreateProviderRequest(provider.Name, template, message);
            var result = await SendWithPolicyAsync(provider, providerRequest, cancellationToken);

            lastResult = result;
            lastProvider = provider.Name;

            if (result.Succeeded)
            {
                return new SmsSendResult(true, provider.Name, result.ProviderMessageId, null, null);
            }

            if (!result.CanFallback)
            {
                break;
            }
        }

        var errorCode = lastResult?.ErrorCode ?? "sms_provider_unavailable";
        var errorMessage = lastResult?.ErrorMessage ?? "No available sms provider.";

        return new SmsSendResult(false, lastProvider, null, errorCode, errorMessage);
    }

    private async Task<SmsProviderResult> SendWithPolicyAsync(
        ISmsProvider provider,
        SmsProviderRequest request,
        CancellationToken cancellationToken)
    {
        var timeout = Policy.TimeoutAsync<SmsProviderResult>(
            TimeSpan.FromSeconds(options.Value.TimeoutSeconds),
            TimeoutStrategy.Pessimistic);
        var retry = Policy<SmsProviderResult>
            .Handle<HttpRequestException>()
            .Or<TimeoutRejectedException>()
            .OrResult(result => !result.Succeeded && result.IsRetryable)
            .WaitAndRetryAsync(
                options.Value.RetryCount,
                retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt));

        var policy = Policy.WrapAsync(retry, timeout);

        try
        {
            return await policy.ExecuteAsync(
                token => provider.SendAsync(request, token),
                cancellationToken);
        }
        catch (TimeoutRejectedException ex)
        {
            logger.LogWarning(ex, "Sms provider timed out. Provider={Provider}, TraceId={TraceId}", provider.Name, request.TraceId);

            return SmsProviderResult.Failed("timeout", "Sms provider request timed out.", true, false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Sms provider http request failed. Provider={Provider}, TraceId={TraceId}", provider.Name, request.TraceId);

            return SmsProviderResult.Failed("http_request_failed", ex.Message, true, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Sms provider request failed. Provider={Provider}, TraceId={TraceId}", provider.Name, request.TraceId);

            return SmsProviderResult.Failed("provider_exception", ex.Message, false, false);
        }
    }

    private SmsTemplateOptions GetTemplate(string templateName)
    {
        return options.Value.Templates.TryGetValue(templateName, out var template)
            ? template
            : throw new InvalidOperationException($"Sms Template is not configured. TemplateName={templateName}");
    }

    private string[] GetProviderNames()
    {
        if (options.Value.FallbackEnabled && options.Value.FallbackProviders.Length > 0)
        {
            return options.Value.FallbackProviders;
        }

        return [options.Value.DefaultProvider];
    }

    private SmsProviderRequest CreateProviderRequest(
        string providerName,
        SmsTemplateOptions template,
        SmsMessage request)
    {
        var templateCode = providerName.Equals(SmsProviderNames.Tencent, StringComparison.OrdinalIgnoreCase)
            ? template.TencentTemplateId
            : template.AliyunTemplateCode;

        if (providerName.Equals(SmsProviderNames.Fake, StringComparison.OrdinalIgnoreCase))
        {
            templateCode = request.TemplateName;
        }

        return new SmsProviderRequest(
            request.PhoneNumber,
            options.Value.SignName,
            templateCode,
            template.Content,
            request.Parameters,
            template.ParameterOrder,
            request.Scene,
            request.RequestId,
            request.TraceId);
    }
}
