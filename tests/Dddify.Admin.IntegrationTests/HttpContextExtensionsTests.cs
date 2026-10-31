using Dddify.Admin.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;

namespace Dddify.Admin.IntegrationTests;

public class HttpContextExtensionsTests
{
    [Theory]
    [InlineData("203.0.113.10", "203.0.113.10")]
    [InlineData("2001:db8::10", "2001:db8::10")]
    [InlineData("::ffff:203.0.113.10", "203.0.113.10")]
    public void Direct_Requests_Return_Normalized_Client_Address(string address, string expected)
    {
        var context = CreateContext(address);

        Assert.Equal(expected, context.GetClientIpAddress());
    }

    [Fact]
    public void Missing_Request_Information_Has_Explicit_Defaults()
    {
        var context = new DefaultHttpContext();

        Assert.Null(context.GetClientIpAddress());
        Assert.Equal(string.Empty, context.GetUserAgent());
        Assert.Equal(string.Empty, context.GetDeviceId());
    }

    [Fact]
    public void UserAgent_And_DeviceId_Are_Read_From_Request_Headers()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.UserAgent = "Browser/1.0 (Test)";
        context.Request.Headers["X-Device-Id"] = "device";

        Assert.Equal("Browser/1.0 (Test)", context.GetUserAgent());
        Assert.Equal("device", context.GetDeviceId());
    }

    [Fact]
    public async Task Trusted_Proxy_Forwards_Client_Address_And_Scheme()
    {
        var context = CreateContext("10.20.0.10", "198.51.100.42:54321", "https");
        context.Request.Headers.UserAgent = "Browser/1.0";
        var options = CreateOptions();
        options.KnownProxies.Add(IPAddress.Parse("10.20.0.10"));

        await ApplyForwardingAsync(context, options);

        Assert.Equal("198.51.100.42", context.GetClientIpAddress());
        Assert.Equal("https", context.Request.Scheme);
        Assert.Equal("Browser/1.0", context.GetUserAgent());
    }

    [Fact]
    public async Task Unknown_Proxy_Cannot_Override_Client_Address()
    {
        var context = CreateContext("198.51.100.10", "203.0.113.42", "https");
        context.Request.Headers["X-Real-IP"] = "203.0.113.99";

        await ApplyForwardingAsync(context, CreateOptions());

        Assert.Equal("198.51.100.10", context.GetClientIpAddress());
        Assert.Equal("http", context.Request.Scheme);
    }

    [Theory]
    [InlineData(1, "10.20.0.10")]
    [InlineData(2, "203.0.113.42")]
    public async Task Multiple_Proxies_Respect_Configured_Forward_Limit(int limit, string expected)
    {
        var context = CreateContext("10.20.0.20", "203.0.113.42, 10.20.0.10", "https, http");
        var options = CreateOptions(limit);
        options.KnownProxies.Add(IPAddress.Parse("10.20.0.10"));
        options.KnownProxies.Add(IPAddress.Parse("10.20.0.20"));

        await ApplyForwardingAsync(context, options);

        Assert.Equal(expected, context.GetClientIpAddress());
    }

    [Fact]
    public async Task Proxy_Chain_Stops_At_An_Untrusted_Intermediate_Address()
    {
        var context = CreateContext("10.20.0.20", "203.0.113.42, 198.51.100.10", "https, http");
        var options = CreateOptions(2);
        options.KnownProxies.Add(IPAddress.Parse("10.20.0.20"));

        await ApplyForwardingAsync(context, options);

        Assert.Equal("198.51.100.10", context.GetClientIpAddress());
    }

    [Fact]
    public async Task Trusted_Ipv6_Proxy_Network_Accepts_Ipv6_Client_Addresses()
    {
        var context = CreateContext("fd00:1::2", "[2001:db8::7]:443", "https");
        var options = CreateOptions();
        options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse("fd00:1::/64"));

        await ApplyForwardingAsync(context, options);

        Assert.Equal("2001:db8::7", context.GetClientIpAddress());
    }

    [Fact]
    public async Task Loopback_Proxy_Is_Trusted_By_Default()
    {
        var context = CreateContext("127.0.0.1", "203.0.113.42", "https");

        await ApplyForwardingAsync(context, CreateOptions());

        Assert.Equal("203.0.113.42", context.GetClientIpAddress());
    }

    [Fact]
    public async Task Malformed_Forwarded_Address_Does_Not_Replace_Connection_Address()
    {
        var context = CreateContext("127.0.0.1", "not-an-ip", "https");

        await ApplyForwardingAsync(context, CreateOptions());

        Assert.Equal("127.0.0.1", context.GetClientIpAddress());
    }

    private static DefaultHttpContext CreateContext(string address, string? forwardedFor = null, string? forwardedProto = null)
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(address);
        context.Request.Scheme = "http";
        if (forwardedFor is not null) context.Request.Headers["X-Forwarded-For"] = forwardedFor;
        if (forwardedProto is not null) context.Request.Headers["X-Forwarded-Proto"] = forwardedProto;
        return context;
    }

    private static ForwardedHeadersOptions CreateOptions(int limit = 1) => new()
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        ForwardLimit = limit
    };

    private static Task ApplyForwardingAsync(HttpContext context, ForwardedHeadersOptions options) =>
        new ForwardedHeadersMiddleware(_ => Task.CompletedTask, NullLoggerFactory.Instance, Options.Create(options)).Invoke(context);
}
