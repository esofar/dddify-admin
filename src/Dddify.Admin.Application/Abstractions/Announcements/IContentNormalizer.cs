using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Abstractions.Announcements;

public interface IContentNormalizer
{
    AnnouncementContent Normalize(string html);
}
