using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Infrastructure.Repositories;

public class InboxItemRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, InboxItem, Guid>(context), IInboxItemRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<InboxItem?> GetUserInboxItemAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.InboxItems
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId, cancellationToken);
    }

    public async Task<int> CountUserUnreadInboxItemsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.InboxItems
            .AsNoTracking()
            .CountAsync(i => i.UserId == userId && !i.IsRead, cancellationToken);
    }
}
