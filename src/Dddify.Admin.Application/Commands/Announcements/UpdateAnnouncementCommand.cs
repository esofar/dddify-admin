using Dddify.Admin.Application.Exceptions.Announcements;
using Dddify.Admin.Application.Services.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Commands.Announcements;

public record UpdateAnnouncementCommand(
    Guid Id,
    string Title,
    string Summary,
    string ContentHtml,
    string AudienceType,
    IEnumerable<Guid>? AudienceTargetIds) : ICommand;

public class UpdateAnnouncementCommandValidator : AbstractValidator<UpdateAnnouncementCommand>
{
    public UpdateAnnouncementCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty();

        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(Announcement.MaxTitleLength);

        RuleFor(c => c.Summary)
            .NotEmpty()
            .MaximumLength(Announcement.MaxSummaryLength);

        RuleFor(c => c.ContentHtml)
            .NotEmpty()
            .MaximumLength(AnnouncementContent.MaxHtmlLength);

        RuleFor(c => c.AudienceType)
            .NotEmpty()
            .IsEnumName(typeof(AnnouncementAudienceType), caseSensitive: false);

        RuleFor(c => c.AudienceTargetIds)
           .NotEmpty()
           .When(c => c.AudienceType != nameof(AnnouncementAudienceType.AllUsers));
    }
}

public class UpdateAnnouncementCommandHandler(
    IAnnouncementRepository announcementRepository,
    IContentNormalizer contentNormalizer) : ICommandHandler<UpdateAnnouncementCommand>
{
    public async Task Handle(UpdateAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new AnnouncementNotFoundException(command.Id);

        var audienceType = Enum.Parse<AnnouncementAudienceType>(command.AudienceType, ignoreCase: true);
        var announcementAudience = new AnnouncementAudience(audienceType, command.AudienceTargetIds ?? []);
        var announcementContent = contentNormalizer.Normalize(command.ContentHtml);

        announcement.Change(
            command.Title,
            command.Summary,
            announcementContent,
            announcementAudience);
    }
}
