using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Infrastructure.Persistence.Repositories;

public class AnnouncementRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, Announcement, Guid>(context), IAnnouncementRepository
{
}
