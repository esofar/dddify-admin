using Dddify.Admin.Application.Commands.Auths;
using Dddify.Admin.Application.Dtos.Auths;
using Dddify.Admin.Web.Controllers;
using Dddify.Admin.Web.Requests.Auths;
using Dddify.Timing;
using Dddify.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Dddify.Admin.IntegrationTests;

public class AuthControllerTests
{
    [Theory]
    [InlineData("password", true)]
    [InlineData("password", false)]
    [InlineData("verification-code", true)]
    [InlineData("verification-code", false)]
    [InlineData("refresh", true)]
    [InlineData("refresh", false)]
    public async Task Authentication_Uses_Lifetime_From_Command_Result_For_Cookies(string flow, bool persistent)
    {
        var now = new DateTimeOffset(2026, 10, 31, 0, 0, 0, TimeSpan.Zero);
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "__Host-refresh_token=old-refresh-token";
        context.Request.Headers["X-Device-Id"] = "device";
        var commandType = flow switch
        {
            "password" => typeof(LoginWithPasswordCommand),
            "verification-code" => typeof(LoginWithVerificationCodeCommand),
            _ => typeof(RefreshTokenCommand)
        };
        var sender = Stub<ISender>((method, args) =>
        {
            Assert.Equal("Send", method.Name);
            Assert.Equal(commandType, args![0]!.GetType());
            if (args[0] is LoginWithPasswordCommand passwordLogin)
            {
                Assert.Equal(persistent, passwordLogin.IsPersistent);
            }
            if (args[0] is LoginWithVerificationCodeCommand verificationCodeLogin)
            {
                Assert.Equal(persistent, verificationCodeLogin.IsPersistent);
            }
            return Task.FromResult(new TokenPairDto("access-token", "refresh-token", persistent, 14));
        });
        var clock = Stub<IClock>((method, _) => method.Name == "get_UtcNow" ? now : throw new InvalidOperationException());
        var currentUser = Stub<ICurrentUser>((_, _) => throw new InvalidOperationException());
        var controller = new AuthController(sender, clock, currentUser)
        {
            ControllerContext = new ControllerContext { HttpContext = context }
        };

        var token = flow switch
        {
            "password" => await controller.LoginWithPasswordAsync(
                new PasswordLoginRequest("member@example.com", "password", "device", "browser", persistent), default),
            "verification-code" => await controller.LoginWithVerificationCodeAsync(
                new VerificationCodeLoginRequest("member@example.com", "012345", "device", "browser", persistent), default),
            _ => await controller.RefreshTokenAsync(default)
        };

        Assert.Equal("access-token", token);
        var cookie = Assert.Single(context.Response.GetTypedHeaders().SetCookie!);
        Assert.Equal("__Host-refresh_token", cookie.Name.Value);
        Assert.Equal("refresh-token", cookie.Value.Value);
        Assert.True(cookie.HttpOnly);
        Assert.True(cookie.Secure);
        Assert.Equal("/", cookie.Path.Value);
        if (persistent)
        {
            Assert.Equal(TimeSpan.FromDays(14), cookie.MaxAge);
            Assert.Equal(now.AddDays(14), cookie.Expires);
        }
        else
        {
            Assert.Null(cookie.MaxAge);
            Assert.Null(cookie.Expires);
        }
    }

    private static T Stub<T>(Func<MethodInfo, object?[]?, object?> callback) where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).Callback = callback;
        return proxy;
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[]?, object?> Callback { get; set; } = default!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Callback(method!, args);
    }
}
