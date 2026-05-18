using System.ComponentModel.DataAnnotations;

namespace Dddify.Admin.Application.Common.Sms;

public sealed class SmsOptions
{
    public const string SectionName = "Sms";

    public string DefaultProvider { get; set; } = SmsProviderNames.Fake;

    public bool FallbackEnabled { get; set; } = false;

    public string[] FallbackProviders { get; set; } = [];

    public string SignName { get; set; } = string.Empty;

    [Range(1, 30)]
    public int TimeoutSeconds { get; set; } = 3;

    [Range(0, 5)]
    public int RetryCount { get; set; } = 1;

    public Dictionary<string, SmsTemplateOptions> Templates { get; set; } = [];

    public SmsVerificationOptions Verification { get; set; } = new();

    public SmsRateLimitOptions RateLimit { get; set; } = new();

    public FakeSmsProviderOptions Fake { get; set; } = new();

    public AliyunSmsProviderOptions Aliyun { get; set; } = new();

    public TencentSmsProviderOptions Tencent { get; set; } = new();
}

public sealed class SmsTemplateOptions
{
    public string Content { get; set; } = string.Empty;

    public string AliyunTemplateCode { get; set; } = string.Empty;

    public string TencentTemplateId { get; set; } = string.Empty;

    public string[] ParameterOrder { get; set; } = [];
}

public sealed class SmsVerificationOptions
{
    [Range(4, 8)]
    public int CodeLength { get; set; } = 6;

    [Range(1, 30)]
    public int ExpiresMinutes { get; set; } = 5;

    [Range(1, 600)]
    public int CooldownSeconds { get; set; } = 60;

    [Range(1, 50)]
    public int MaxSendsPerWindow { get; set; } = 5;

    [Range(1, 1440)]
    public int SendWindowMinutes { get; set; } = 60;

    [Range(1, 20)]
    public int MaxVerifyAttempts { get; set; } = 5;

    [Range(1, 1440)]
    public int VerifyWindowMinutes { get; set; } = 10;

    public string LoginTemplateName { get; set; } = "LoginCode";

    public string HashSecret { get; set; } = string.Empty;
}

public sealed class SmsRateLimitOptions
{
    public bool Enabled { get; set; } = true;

    [Range(1, 500)]
    public int MaxSendsPerPhone { get; set; } = 20;

    [Range(1, 86_400)]
    public int PerPhoneWindowSeconds { get; set; } = 3600;

    [Range(1, 5_000)]
    public int MaxSendsPerIp { get; set; } = 100;

    [Range(1, 86_400)]
    public int PerIpWindowSeconds { get; set; } = 3600;
}

public sealed class FakeSmsProviderOptions
{
    public bool LogMessageContent { get; set; } = true;

    [Range(0, 10_000)]
    public int LatencyMilliseconds { get; set; }
}

public sealed class AliyunSmsProviderOptions
{
    public string AccessKeyId { get; set; } = string.Empty;

    public string AccessKeySecret { get; set; } = string.Empty;

    public string Endpoint { get; set; } = "dysmsapi.aliyuncs.com";
}

public sealed class TencentSmsProviderOptions
{
    public string SecretId { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string Region { get; set; } = "ap-guangzhou";

    public string Endpoint { get; set; } = "sms.tencentcloudapi.com";

    public string SmsSdkAppId { get; set; } = string.Empty;
}
