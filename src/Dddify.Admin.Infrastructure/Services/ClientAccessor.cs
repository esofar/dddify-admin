using Microsoft.AspNetCore.Http;

namespace Dddify.Admin.Infrastructure.Services;

[ScopedDependency(RegistrationMode.AsMatchingInterface)]
public class ClientAccessor(IHttpContextAccessor httpContextAccessor) : IClientAccessor
{
    private HttpContext? HttpContext => httpContextAccessor.HttpContext;

    public string? IpAddress => HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? UserAgent => HttpContext?.Request.Headers.UserAgent.ToString();

    public string? TraceId => HttpContext?.TraceIdentifier;
}