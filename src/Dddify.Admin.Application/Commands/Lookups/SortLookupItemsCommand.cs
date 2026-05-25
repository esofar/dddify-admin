using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record SortLookupItemsCommand(
    Guid Id,
    Guid[] OrderedItemIds) : ICommand;

public class SortLookupItemsCommandValidator : AbstractValidator<SortLookupItemsCommand>
{
    public SortLookupItemsCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.OrderedItemIds)
            .NotEmpty();
    }
}

public class SortLookupItemsCommandHandler(
    ILookupRepository lookupRepository,
    IDistributedCache distributedCache) : ICommandHandler<SortLookupItemsCommand>
{
    public async Task Handle(SortLookupItemsCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.Id, cancellationToken)
            ?? throw new LookupNotFoundException(command.Id);

        lookup.SortItems(command.OrderedItemIds);

        await distributedCache.RemoveAsync(CacheKeys.Lookup.Items(lookup.Code), cancellationToken);
    }
}
