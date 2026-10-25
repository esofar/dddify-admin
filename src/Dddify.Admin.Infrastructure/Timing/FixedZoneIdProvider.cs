namespace Dddify.Admin.Infrastructure.Timing;

public class FixedZoneIdProvider : ITimeZoneIdProvider
{
    public string? GetTimeZoneId() => "Asia/Shanghai";
}