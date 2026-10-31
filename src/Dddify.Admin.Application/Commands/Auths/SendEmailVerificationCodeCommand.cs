using Dddify.Admin.Application.Abstractions.Email;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Auths;

[SkipUnitOfWorkBehavior]
public record SendEmailVerificationCodeCommand(string Email, string? IpAddress) : ICommand;

public class SendEmailVerificationCodeCommandValidator : AbstractValidator<SendEmailVerificationCodeCommand>
{
    public SendEmailVerificationCodeCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(User.MaxEmailLength)
            .EmailAddress();
    }
}

public class SendEmailVerificationCodeCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationCodeService codeService,
    IEmailSender emailSender) : ICommandHandler<SendEmailVerificationCodeCommand>
{
    public async Task Handle(SendEmailVerificationCodeCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        // 未注册和禁用账号使用相同响应与限流规则。
        var code = await codeService.CreateAsync(email, command.IpAddress, cancellationToken);
        var user = await userRepository.GetByEmailAsync(email);
        if (user is null || user.Status != UserStatus.Enabled)
        {
            await codeService.RemoveAsync(email, code.Value, cancellationToken);
            return;
        }

        try
        {
            await emailSender.SendAsync(
                new EmailMessage(
                    user.Email,
                    "登录验证码 / Sign-in verification code",
                    $"您的登录验证码为 {code.Value}，{code.ExpiresMinutes} 分钟内有效，请勿泄露。\n" +
                    $"Your sign-in code is {code.Value}. It expires in {code.ExpiresMinutes} minutes.")
                {
                    IsHtml = false
                },
                cancellationToken);
        }
        catch
        {
            await codeService.RemoveAsync(email, code.Value, CancellationToken.None);
            throw;
        }
    }
}
