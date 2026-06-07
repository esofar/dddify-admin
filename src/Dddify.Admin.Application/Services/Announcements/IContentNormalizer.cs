using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Services.Announcements;

public interface IContentNormalizer
{
    AnnouncementContent Normalize(string html);
}
