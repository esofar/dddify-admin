using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dysmsapi20170525;
using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using System.Text.Json;

namespace Dddify.Admin.Infrastructure.Sms.Providers;

public sealed class AliyunSmsProvider(IOptions<SmsOptions> options) : ISmsProvider
{
    public string Name => SmsProviderNames.Aliyun;

    public async Task<SmsProviderResult> SendAsync(SmsProviderRequest request, CancellationToken cancellationToken)
    {
        var aliyun = options.Value.Aliyun;

        if (string.IsNullOrWhiteSpace(aliyun.AccessKeyId) || string.IsNullOrWhiteSpace(aliyun.AccessKeySecret))
        {
            return SmsProviderResult.Failed("provider_not_configured", "Aliyun sms provider is not configured.", false, true);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var client = new Client(new Config
        {
            AccessKeyId = aliyun.AccessKeyId,
            AccessKeySecret = aliyun.AccessKeySecret,
            Endpoint = aliyun.Endpoint
        });

        var sendRequest = new SendSmsRequest
        {
            PhoneNumbers = request.PhoneNumber,
            SignName = request.SignName,
            TemplateCode = request.TemplateCode,
            TemplateParam = JsonSerializer.Serialize(request.Parameters)
        };

        var response = await client.SendSmsAsync(sendRequest);
        var body = response.Body;

        if (body is null)
        {
            return SmsProviderResult.Failed("empty_response", "Aliyun sms returned empty response.", true, true);
        }

        if (string.Equals(body.Code, "OK", StringComparison.OrdinalIgnoreCase))
        {
            return SmsProviderResult.Success(body.BizId);
        }

        return SmsProviderResult.Failed(
            body.Code,
            body.Message,
            IsRetryable(body.Code),
            CanFallback(body.Code));
    }

    private static bool IsRetryable(string? code)
    {
        return string.IsNullOrWhiteSpace(code) ||
            code.Contains("SYSTEM", StringComparison.OrdinalIgnoreCase) ||
            code.Contains("THROTTLING", StringComparison.OrdinalIgnoreCase) ||
            code.Contains("LIMIT", StringComparison.OrdinalIgnoreCase);
    }

    private static bool CanFallback(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return true;
        }

        return IsRetryable(code);
    }
}
