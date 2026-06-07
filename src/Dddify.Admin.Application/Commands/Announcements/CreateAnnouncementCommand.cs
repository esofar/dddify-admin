using Dddify.Admin.Application.Services.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Commands.Announcements;

public record CreateAnnouncementCommand(
    string Title,
    string Summary,
    string ContentHtml,
    string AudienceType,
    IEnumerable<Guid>? AudienceTargetIds) : ICommand;

public class CreateAnnouncementCommandValidator : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementCommandValidator()
    {
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

public class CreateAnnouncementCommandHandler(
    IAnnouncementRepository announcementRepository,
    IContentNormalizer contentNormalizer,
    IGuidGenerator guidGenerator) : ICommandHandler<CreateAnnouncementCommand>
{
    public async Task Handle(CreateAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var audienceType = Enum.Parse<AnnouncementAudienceType>(command.AudienceType, ignoreCase: true);
        var announcementAudience = new AnnouncementAudience(audienceType, command.AudienceTargetIds ?? []);
        var announcementContent = contentNormalizer.Normalize(command.ContentHtml);

        var announcement = new Announcement(
            guidGenerator.Create(),
            command.Title,
            command.Summary,
            announcementContent,
            announcementAudience);

        await announcementRepository.AddAsync(announcement, cancellationToken);
    }
}
