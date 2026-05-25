using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record DeleteLookupItemCommand(
    Guid LookupId,
    Guid LookupItemId) : ICommand;

public class DeleteLookupItemCommandValidator : AbstractValidator<DeleteLookupItemCommand>
{
    public DeleteLookupItemCommandValidator()
    {
        RuleFor(x => x.LookupId)
            .NotEmpty();

        RuleFor(x => x.LookupItemId)
            .NotEmpty();
    }
}

public class DeleteLookupItemCommandHandler(
    ILookupRepository lookupRepository,
    IDistributedCache distributedCache) : ICommandHandler<DeleteLookupItemCommand>
{
    public async Task Handle(DeleteLookupItemCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(command.LookupId);

        lookup.RemoveItem(command.LookupItemId);

        await distributedCache.RemoveAsync(CacheKeys.Lookup.Items(lookup.Code), cancellationToken);
    }
}
