using Dddify.Admin.Domain.Aggregates.InboxItems;

namespace Dddify.Admin.Domain.Repositories;

public interface IInboxItemRepository : IRepository<InboxItem, Guid>
{
    Task<InboxItem?> GetUserInboxItemAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<int> CountUserUnreadInboxItemsAsync(Guid userId, CancellationToken cancellationToken = default);
}
