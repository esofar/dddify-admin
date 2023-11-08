using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record DeleteDictionaryItemCommand(Guid DictionaryId, Guid DictionaryItemId) : ICommand;

public class DeleteDictionaryItemCommandValidator : AbstractValidator<DeleteDictionaryItemCommand>
{
    public DeleteDictionaryItemCommandValidator()
    {
        RuleFor(c => c.DictionaryId).NotEmpty();
        RuleFor(c => c.DictionaryItemId).NotEmpty();
    }
}

public class DeleteDictionaryItemCommandHandler : ICommandHandler<DeleteDictionaryItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDictionaryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteDictionaryItemCommand command, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.DictionaryId, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(command.DictionaryId);
        }

        dictionary.DeleteItem(command.DictionaryItemId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}