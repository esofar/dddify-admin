using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record DeleteLookupCommand(Guid Id) : ICommand;

public class DeleteLookupCommandHandler(
    ILookupRepository lookupRepository,
    IDistributedCache distributedCache) : ICommandHandler<DeleteLookupCommand>
{
    public async Task Handle(DeleteLookupCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.Id, cancellationToken)
            ?? throw new LookupNotFoundException(command.Id);

        lookup.EnsureCanDelete();

        lookupRepository.Remove(lookup);

        await distributedCache.RemoveAsync(CacheKeys.Lookup.Items(lookup.Code), cancellationToken);
    }
}