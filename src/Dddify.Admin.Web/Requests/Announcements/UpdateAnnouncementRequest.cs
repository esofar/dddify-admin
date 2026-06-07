namespace Dddify.Admin.Web.Requests.Announcements;

/// <summary>
/// Update announcement request.
/// </summary>
/// <param name="Title">Announcement title.</param>
/// <param name="Summary">Announcement summary.</param>
/// <param name="ContentHtml">Announcement content HTML.</param>
/// <param name="AudienceType">Announcement audience type.</param>
/// <param name="AudienceTargetIds">Announcement audience target ids.</param>
public record UpdateAnnouncementRequest(
    string Title,
    string Summary,
    string ContentHtml,
    string AudienceType,
    IEnumerable<Guid>? AudienceTargetIds);
