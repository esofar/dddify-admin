using Dddify.Admin.Application.Exceptions.Lookups;
using Dddify.Admin.Domain.Aggregates.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record UpdateLookupItemCommand(Guid LookupId, Guid ItemId, string Label, string? Color) : ICommand;

public class UpdateLookupItemCommandValidator : AbstractValidator<UpdateLookupItemCommand>
{
    public UpdateLookupItemCommandValidator()
    {
        RuleFor(x => x.LookupId)
            .NotEmpty();

        RuleFor(x => x.ItemId)
            .NotEmpty();

        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(LookupItem.MaxLabelLength);

        RuleFor(x => x.Color)
            .MaximumLength(LookupItem.MaxColorLength);
    }
}

public class UpdateLookupItemCommandHandler(ILookupRepository lookupRepository, IDistributedCache distributedCache) : ICommandHandler<UpdateLookupItemCommand>
{
    public async Task Handle(UpdateLookupItemCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(command.LookupId);

        lookup.ChangeItem(command.ItemId, command.Label, command.Color);

        distributedCache.Remove(CacheKeys.Lookup.Items(lookup.Code));
    }
}
