using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record SortDictionaryItemCommand(Guid DictionaryId, Guid[] DictionaryItemIds) : ICommand;

public class SortDictionaryItemCommandValidator : AbstractValidator<SortDictionaryItemCommand>
{
    public SortDictionaryItemCommandValidator()
    {
        RuleFor(c => c.DictionaryId).NotEmpty();
        RuleFor(c => c.DictionaryItemIds).NotEmpty();
    }
}

public class SortDictionaryItemCommandHandler : ICommandHandler<SortDictionaryItemCommand>
{
    private readonly IApplicationDbContext _context;

    public SortDictionaryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SortDictionaryItemCommand command, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.DictionaryId, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(command.DictionaryId);
        }

        dictionary.SortItems(command.DictionaryItemIds);

        await _context.SaveChangesAsync(cancellationToken);
    }
}