using Dddify.Admin.Domain.Aggregates.Lookups;

namespace Dddify.Admin.Infrastructure.Repositories;

public class LookupRepository(ApplicationDbContext context) : RepositoryBase<ApplicationDbContext, Lookup, Guid>(context), ILookupRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Lookup?> GetLookupWithItemsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Lookups
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Code == code, cancellationToken);
    }

    public async Task<Lookup?> GetLookupWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lookups
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }
}