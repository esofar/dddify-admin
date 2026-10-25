using Dddify.Admin.Application.Abstractions.Email;
using Dddify.Admin.Application.Configuration.Email;
using Dddify.Admin.Domain.Aggregates.Users;
using Microsoft.Extensions.Options;

namespace Dddify.Admin.Application.Commands.Auths;

[SkipUnitOfWorkBehavior]
public record SendEmailVerificationCodeCommand(string Email, string? IpAddress) : ICommand;

public class SendEmailVerificationCodeCommandValidator : AbstractValidator<SendEmailVerificationCodeCommand>
{
    public SendEmailVerificationCodeCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().MaximumLength(User.MaxEmailLength).EmailAddress();
    }
}

public class SendEmailVerificationCodeCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationCodeService codeService,
    IEmailSender emailSender,
    IOptions<EmailVerificationCodeOptions> options) : ICommandHandler<SendEmailVerificationCodeCommand>
{
    public async Task Handle(SendEmailVerificationCodeCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        // Unknown and disabled accounts receive the same response and rate limits.
        var code = await codeService.CreateAsync(email, command.IpAddress, cancellationToken);
        var user = await userRepository.GetByEmailAsync(email);
        if (user is null || user.Status != UserStatus.Enabled)
        {
            await codeService.RemoveAsync(email, code, cancellationToken);
            return;
        }

        try
        {
            await emailSender.SendAsync(
                new EmailMessage(
                    user.Email,
                    "登录验证码 / Sign-in verification code",
                    $"您的登录验证码为 {code}，{options.Value.ExpiresMinutes} 分钟内有效，请勿泄露。\n" +
                    $"Your sign-in code is {code}. It expires in {options.Value.ExpiresMinutes} minutes.")
                {
                    IsHtml = false
                },
                cancellationToken);
        }
        catch
        {
            await codeService.RemoveAsync(email, code, CancellationToken.None);
            throw;
        }
    }
}
