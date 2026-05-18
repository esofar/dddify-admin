namespace Dddify.Admin.Infrastructure.Services;

public class FixedZoneIdProvider : ITimeZoneIdProvider
{
    public string? GetTimeZoneId() => "Asia/Shanghai";
}