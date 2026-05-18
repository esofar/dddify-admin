namespace Dddify.Admin.Infrastructure.Session;

public sealed class SessionCleanupOptions
{
    public const string SectionName = "SessionCleanup";

    [Range(1, 1440)]
    public int IntervalMinutes { get; set; } = 60;

    [Range(0, 365)]
    public int RevokedRetentionDays { get; set; } = 30;

    public bool RunOnStartup { get; init; } = false;
}
