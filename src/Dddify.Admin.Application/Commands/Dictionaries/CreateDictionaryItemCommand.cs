using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record CreateDictionaryItemCommand(
    Guid DictionaryId,
    string Code,
    string Name) : ICommand;

public class CreateDictionaryItemCommandValidator : AbstractValidator<CreateDictionaryItemCommand>
{
    public CreateDictionaryItemCommandValidator()
    {
        RuleFor(c => c.DictionaryId).NotEmpty();
        RuleFor(c => c.Code).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
    }
}

public class CreateDictionaryItemCommandHandler : ICommandHandler<CreateDictionaryItemCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDictionaryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateDictionaryItemCommand command, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.DictionaryId, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(command.DictionaryId);
        }

        dictionary.AddItem(command.Code, command.Name, DictionaryItemType.Custom);

        await _context.SaveChangesAsync(cancellationToken);
    }
}