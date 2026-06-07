using Dddify.Admin.Application.Exceptions.Announcements;

namespace Dddify.Admin.Application.Commands.Announcements;

public record WithdrawAnnouncementCommand(Guid Id) : ICommand;

public class WithdrawAnnouncementCommandValidator : AbstractValidator<WithdrawAnnouncementCommand>
{
    public WithdrawAnnouncementCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}

public class WithdrawAnnouncementCommandHandler(
    IAnnouncementRepository announcementRepository,
    ICurrentUser currentUser,
    IClock clock) : ICommandHandler<WithdrawAnnouncementCommand>
{
    public async Task Handle(WithdrawAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new AnnouncementNotFoundException(command.Id);

        announcement.Withdraw(currentUser.GetIdAsGuid(), clock.UtcNow);
    }
}
