using Dddify.Admin.Application.Exceptions.Announcements;

namespace Dddify.Admin.Application.Commands.Announcements;

public record PublishAnnouncementCommand(Guid Id) : ICommand;

public class PublishAnnouncementCommandValidator : AbstractValidator<PublishAnnouncementCommand>
{
    public PublishAnnouncementCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}

public class PublishAnnouncementCommandHandler(
    IAnnouncementRepository announcementRepository,
    ICurrentUser currentUser,
    IClock clock) : ICommandHandler<PublishAnnouncementCommand>
{
    public async Task Handle(PublishAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new AnnouncementNotFoundException(command.Id);

        announcement.Publish(currentUser.GetIdAsGuid(), clock.UtcNow);
    }
}
