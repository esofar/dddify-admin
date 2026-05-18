namespace Dddify.Admin.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public required string Configuration { get; set; }

    public string? InstanceName { get; set; }
}
