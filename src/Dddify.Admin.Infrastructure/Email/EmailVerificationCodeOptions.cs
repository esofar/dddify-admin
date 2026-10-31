using System.ComponentModel.DataAnnotations;

namespace Dddify.Admin.Infrastructure.Email;

public sealed class EmailVerificationCodeOptions
{
    public const string SectionName = "EmailVerificationCode";

    [Required, MinLength(32)]
    public string HashSecret { get; set; } = string.Empty;

    [Range(1, 30)]
    public int ExpiresMinutes { get; set; } = 5;

    [Range(1, 600)]
    public int CooldownSeconds { get; set; } = 60;

    [Range(1, 100)]
    public int MaxSendsPerEmailPerHour { get; set; } = 5;

    [Range(1, 5000)]
    public int MaxSendsPerIpPerHour { get; set; } = 100;

    [Range(1, 20)]
    public int MaxVerifyAttempts { get; set; } = 5;
}
