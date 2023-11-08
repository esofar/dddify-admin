using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record UpdateDictionaryItemCommand(
    Guid DictionaryId,
    Guid DictionaryItemId,
    string Name,
    string ConcurrencyStamp) : ICommand;

public class UpdateDictionaryItemCommandValidator : AbstractValidator<UpdateDictionaryItemCommand>
{
    public UpdateDictionaryItemCommandValidator()
    {
        RuleFor(c => c.DictionaryId).NotEmpty();
        RuleFor(c => c.DictionaryItemId).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.ConcurrencyStamp).NotEmpty();
    }
}

public class UpdateDictionaryItemCommandHandler : ICommandHandler<UpdateDictionaryItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDictionaryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateDictionaryItemCommand command, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.DictionaryId, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(command.DictionaryId);
        }

        var dictionaryItem = dictionary.Items.FirstOrDefault(c => c.Id == command.DictionaryItemId);

        if (dictionaryItem is null)
        {
            throw new DictionaryItemNotFoundException(command.DictionaryItemId);
        }

        if (dictionary.Items.Any(c => c.Id != command.DictionaryItemId && c.Name == command.Name))
        {
            throw new DictionaryItemNameDuplicateException(command.Name);
        }

        _context.ResetConcurrencyStamp(dictionaryItem, command.ConcurrencyStamp);

        dictionaryItem.SetName(command.Name);

        await _context.SaveChangesAsync(cancellationToken);
    }
}