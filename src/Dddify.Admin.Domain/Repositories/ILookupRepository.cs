using Dddify.Admin.Domain.Aggregates.Lookups;

namespace Dddify.Admin.Domain.Repositories;

public interface ILookupRepository : IRepository<Lookup, Guid>
{
    Task<Lookup?> GetLookupWithItemsAsync(string code, CancellationToken cancellationToken = default);
    Task<Lookup?> GetLookupWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}
