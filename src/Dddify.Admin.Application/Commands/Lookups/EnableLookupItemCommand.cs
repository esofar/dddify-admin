using Dddify.Admin.Application.Exceptions.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record EnableLookupItemCommand(
    Guid LookupId,
    Guid LookupItemId) : ICommand;

public class EnableLookupItemCommandValidator : AbstractValidator<EnableLookupItemCommand>
{
    public EnableLookupItemCommandValidator()
    {
        RuleFor(x => x.LookupId)
            .NotEmpty();

        RuleFor(x => x.LookupItemId)
            .NotEmpty();
    }
}

public class EnableLookupItemCommandHandler(
    ILookupRepository lookupRepository,
    IDistributedCache distributedCache) : ICommandHandler<EnableLookupItemCommand>
{
    public async Task Handle(EnableLookupItemCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(command.LookupId);

        lookup.EnableItem(command.LookupItemId);

        await distributedCache.RemoveAsync(CacheKeys.Lookup.Items(lookup.Code), cancellationToken);
    }
}
