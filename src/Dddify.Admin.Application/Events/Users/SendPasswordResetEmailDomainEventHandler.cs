using Dddify.Admin.Domain.Events.Users;

namespace Dddify.Admin.Application.Events.Users;

public class SendPasswordResetEmailDomainEventHandler(
    IEmailSender emailSender,
    IStringLocalizer localizer) : IDomainEventHandler<UserPasswordResetDomainEvent>
{
    public async Task Handle(UserPasswordResetDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = localizer["user_password_reset_email_subject"];
        var body = localizer["user_password_reset_email_body", @event.NewPassword];

        await emailSender.SendAsync(@event.Email, subject, body);
    }
}
