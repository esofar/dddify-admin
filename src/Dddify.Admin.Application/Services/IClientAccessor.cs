namespace Dddify.Admin.Application.Services;

public interface IClientAccessor
{
    string? IpAddress { get; }

    string? UserAgent { get; }

    string? TraceId { get; }
}
