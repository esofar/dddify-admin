using Dddify.Admin.Application.Exceptions.Announcements;

namespace Dddify.Admin.Application.Commands.Announcements;

public record DeleteAnnouncementCommand(Guid Id) : ICommand;

public class DeleteAnnouncementCommandValidator : AbstractValidator<DeleteAnnouncementCommand>
{
    public DeleteAnnouncementCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();
    }
}

public class DeleteAnnouncementCommandHandler(IAnnouncementRepository announcementRepository)
    : ICommandHandler<DeleteAnnouncementCommand>
{
    public async Task Handle(DeleteAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new AnnouncementNotFoundException(command.Id);

        announcement.EnsureDelete();

        announcementRepository.Remove(announcement);
    }
}
