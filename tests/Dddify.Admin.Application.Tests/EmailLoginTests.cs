using Dddify.Admin.Application.Commands.Auths;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Application.Abstractions.Authentication;
using Dddify.Admin.Application.Abstractions.Email;
using Dddify.Admin.Application.Configuration.Email;
using Dddify.Admin.Domain.Aggregates.Users;
using Dddify.Admin.Domain.Events.Users;
using Dddify.Admin.Domain.Exceptions.Users;
using Dddify.Admin.Domain.Repositories;
using Dddify.Timing;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Dddify.Admin.Application.Tests;

public class EmailLoginTests
{
    [Theory]
    [InlineData("13800138000", false)]
    [InlineData("not-an-email", false)]
    [InlineData("member@example.com", true)]
    public void PasswordLogin_Requires_Email_Account(string email, bool valid)
    {
        var result = new PasswordLoginCommandValidator().Validate(
            new PasswordLoginCommand(email, "Password123", "device", "browser", null, null, false));
        Assert.Equal(valid, result.IsValid);
    }

    [Theory]
    [InlineData("123456", true)]
    [InlineData("000123", true)]
    [InlineData("12345", false)]
    [InlineData("abcdef", false)]
    public void CodeLogin_Requires_Six_Digits(string code, bool valid)
    {
        Assert.Equal(valid, new VerificationCodeLoginCommandValidator().Validate(LoginCommand(code)).IsValid);
    }

    [Fact]
    public async Task SendCode_Normalizes_Email_And_Sends_To_Registered_Address()
    {
        using var cancellation = new CancellationTokenSource();
        var user = CreateUser();
        string? storedEmail = null;
        string? recipient = null;
        string? body = null;
        var handler = SendHandler(user,
            Stub<IEmailVerificationCodeService>((method, args) =>
            {
                Assert.Equal("CreateAsync", method);
                storedEmail = (string)args![0]!;
                return Task.FromResult("012345");
            }),
            Stub<IEmailSender>((_, args) =>
            {
                var message = Assert.IsType<EmailMessage>(args![0]);
                recipient = message.To;
                body = message.Body;
                Assert.False(message.IsHtml);
                Assert.Equal(cancellation.Token, Assert.IsType<CancellationToken>(args[1]));
                return Task.CompletedTask;
            }));

        await handler.Handle(new SendEmailVerificationCodeCommand(" MEMBER@example.com ", "127.0.0.1"), cancellation.Token);
        Assert.Equal("member@example.com", storedEmail);
        Assert.Equal(user.Email, recipient);
        Assert.Contains("012345", body);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SendCode_Does_Not_Send_For_Unknown_Or_Disabled_Account(bool disabled)
    {
        var user = disabled ? CreateUser() : null;
        user?.Disable();
        var removed = false;
        var service = Stub<IEmailVerificationCodeService>((method, _) => method switch
        {
            "CreateAsync" => Task.FromResult("012345"),
            "RemoveAsync" => Remove(),
            _ => throw new InvalidOperationException(method)
        });
        Task Remove() { removed = true; return Task.CompletedTask; }

        await SendHandler(user, service, Stub<IEmailSender>((method, _) => throw new InvalidOperationException(method)))
            .Handle(new SendEmailVerificationCodeCommand("member@example.com", null), default);
        Assert.True(removed);
    }

    [Fact]
    public async Task SendFailure_Removes_The_Unsent_Code()
    {
        var removed = false;
        Task Remove() { removed = true; return Task.CompletedTask; }
        var service = Stub<IEmailVerificationCodeService>((method, _) => method switch
        {
            "CreateAsync" => Task.FromResult("012345"),
            "RemoveAsync" => Remove(),
            _ => throw new InvalidOperationException(method)
        });
        var handler = SendHandler(CreateUser(), service,
            Stub<IEmailSender>((_, _) => Task.FromException(new InvalidOperationException("delivery failed"))));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new SendEmailVerificationCodeCommand("member@example.com", null), default));
        Assert.True(removed);
    }

    [Fact]
    public async Task ValidCode_Produces_Tokens_And_Login_Event()
    {
        var user = CreateUser();
        var now = DateTimeOffset.UtcNow;
        var tokens = Stub<ITokenService>((method, _) => method switch
        {
            "GenerateAccessToken" => "access-token",
            "GenerateRefreshToken" => "refresh-token",
            "HashRefreshToken" => "refresh-hash",
            "GetRefreshTokenExpiresAt" => now.AddDays(7),
            _ => throw new InvalidOperationException(method)
        });
        var handler = new VerificationCodeLoginCommandHandler(
            Repository(user), Stub<IEmailVerificationCodeService>((_, _) => Task.FromResult(true)), tokens,
            Stub<IClock>((_, _) => now));

        var result = await handler.Handle(LoginCommand("012345"), default);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        Assert.True(result.IsPersistent);
        Assert.Equal(now, user.LastLoginAt);
        Assert.IsType<UserLoginSucceededDomainEvent>(Assert.Single(user.DomainEvents));
    }

    [Fact]
    public async Task InvalidCode_Does_Not_Look_Up_User_Or_Issue_Tokens()
    {
        var handler = new VerificationCodeLoginCommandHandler(
            Stub<IUserRepository>((method, _) => throw new InvalidOperationException(method)),
            Stub<IEmailVerificationCodeService>((_, _) => Task.FromResult(false)),
            Stub<ITokenService>((method, _) => throw new InvalidOperationException(method)),
            Stub<IClock>((method, _) => throw new InvalidOperationException(method)));

        await Assert.ThrowsAsync<UserInvalidEmailVerificationCodeException>(() => handler.Handle(LoginCommand("012345"), default));
    }

    [Fact]
    public async Task DisabledAccount_Cannot_Login_With_A_Valid_Code()
    {
        var user = CreateUser();
        user.Disable();
        var handler = new VerificationCodeLoginCommandHandler(
            Repository(user), Stub<IEmailVerificationCodeService>((_, _) => Task.FromResult(true)),
            Stub<ITokenService>((method, _) => throw new InvalidOperationException(method)),
            Stub<IClock>((method, _) => throw new InvalidOperationException(method)));

        await Assert.ThrowsAsync<UserAlreadyDisabledException>(() => handler.Handle(LoginCommand("012345"), default));
    }

    private static VerificationCodeLoginCommand LoginCommand(string code) =>
        new("member@example.com", code, "device", "browser", null, null, true);

    private static User CreateUser() => new(Guid.NewGuid(), "hash", "Member", null, UserGender.Male,
        null, "member@example.com", "13800138000", Guid.NewGuid(), "Department");

    private static IUserRepository Repository(User? user) => Stub<IUserRepository>((method, args) =>
    {
        Assert.Equal("GetByEmailAsync", method);
        Assert.Equal("member@example.com", args![0]);
        return Task.FromResult(user);
    });

    private static SendEmailVerificationCodeCommandHandler SendHandler(User? user, IEmailVerificationCodeService codes, IEmailSender sender) =>
        new(Repository(user), codes, sender, Options.Create(new EmailVerificationCodeOptions()));

    private static T Stub<T>(Func<string, object?[]?, object?> callback) where T : class
    {
        var proxy = DispatchProxy.Create<T, CallStub>();
        ((CallStub)(object)proxy).Callback = callback;
        return proxy;
    }

    public class CallStub : DispatchProxy
    {
        public Func<string, object?[]?, object?> Callback { get; set; } = default!;
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Callback(targetMethod!.Name, args);
    }
}
