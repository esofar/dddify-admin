using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace Dddify.Admin.Infrastructure.Sms.Providers;

public sealed class TencentSmsProvider(IOptions<SmsOptions> options) : ISmsProvider
{
    public string Name => SmsProviderNames.Tencent;

    public async Task<SmsProviderResult> SendAsync(SmsProviderRequest request, CancellationToken cancellationToken)
    {
        var tencent = options.Value.Tencent;

        if (string.IsNullOrWhiteSpace(tencent.SecretId) ||
            string.IsNullOrWhiteSpace(tencent.SecretKey) ||
            string.IsNullOrWhiteSpace(tencent.SmsSdkAppId))
        {
            return SmsProviderResult.Failed("provider_not_configured", "Tencent sms provider is not configured.", false, true);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var credential = new Credential
        {
            SecretId = tencent.SecretId,
            SecretKey = tencent.SecretKey
        };

        var clientProfile = new ClientProfile
        {
            HttpProfile = new HttpProfile
            {
                Endpoint = tencent.Endpoint
            }
        };

        var client = new SmsClient(credential, tencent.Region, clientProfile);
        var sendRequest = new SendSmsRequest
        {
            PhoneNumberSet = [request.PhoneNumber],
            SmsSdkAppId = tencent.SmsSdkAppId,
            SignName = request.SignName,
            TemplateId = request.TemplateCode,
            TemplateParamSet = [.. request.ParameterOrder.Select(name => request.Parameters.TryGetValue(name, out var value) ? value : string.Empty)]
        };

        var response = await client.SendSms(sendRequest);
        var status = response.SendStatusSet?.FirstOrDefault();

        if (status is null)
        {
            return SmsProviderResult.Failed("empty_response", "Tencent sms returned empty send status.", true, true);
        }

        if (string.Equals(status.Code, "Ok", StringComparison.OrdinalIgnoreCase))
        {
            return SmsProviderResult.Success(status.SerialNo);
        }

        return SmsProviderResult.Failed(
            status.Code,
            status.Message,
            IsRetryable(status.Code),
            CanFallback(status.Code));
    }

    private static bool IsRetryable(string? code)
    {
        return string.IsNullOrWhiteSpace(code) ||
            code.Contains("InternalError", StringComparison.OrdinalIgnoreCase) ||
            code.Contains("RequestLimitExceeded", StringComparison.OrdinalIgnoreCase);
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
