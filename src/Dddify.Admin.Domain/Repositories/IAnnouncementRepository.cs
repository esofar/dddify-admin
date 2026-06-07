using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Domain.Repositories;

public interface IAnnouncementRepository : IRepository<Announcement, Guid>
{
}
