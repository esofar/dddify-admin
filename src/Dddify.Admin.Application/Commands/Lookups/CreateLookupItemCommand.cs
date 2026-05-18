using Dddify.Admin.Application.Exceptions.Lookups;
using Dddify.Admin.Domain.Aggregates.Lookups;

namespace Dddify.Admin.Application.Commands.Lookups;

public record CreateLookupItemCommand(Guid LookupId, string Value, string Label, string? Color) : ICommand;

public class CreateLookupItemCommandValidator : AbstractValidator<CreateLookupItemCommand>
{
    public CreateLookupItemCommandValidator()
    {
        RuleFor(x => x.LookupId)
            .NotEmpty();

        RuleFor(x => x.Value)
            .NotEmpty()
            .MaximumLength(LookupItem.MaxValueLength);

        RuleFor(x => x.Label)
            .NotEmpty()
            .MaximumLength(LookupItem.MaxLabelLength);

        RuleFor(x => x.Color)
            .MaximumLength(LookupItem.MaxColorLength);
    }
}

public class CreateLookupItemCommandHandler(ILookupRepository lookupRepository, IGuidGenerator guidGenerator, IDistributedCache distributedCache) : ICommandHandler<CreateLookupItemCommand>
{
    public async Task Handle(CreateLookupItemCommand command, CancellationToken cancellationToken)
    {
        var lookup = await lookupRepository.GetLookupWithItemsAsync(command.LookupId, cancellationToken)
            ?? throw new LookupNotFoundException(command.LookupId);

        lookup.AddItem(guidGenerator.Create(), command.Value, command.Label, command.Color);

        distributedCache.Remove(CacheKeys.Lookup.Items(lookup.Code));
    }
}
