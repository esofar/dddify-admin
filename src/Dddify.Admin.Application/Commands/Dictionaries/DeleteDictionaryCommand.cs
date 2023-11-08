using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record DeleteDictionaryCommand(Guid Id) : ICommand;

public class DeleteDictionaryCommandValidator : AbstractValidator<DeleteDictionaryCommand>
{
    public DeleteDictionaryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteDictionaryCommandHandler : ICommandHandler<DeleteDictionaryCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteDictionaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteDictionaryCommand command, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(command.Id);
        }

        _context.Dictionaries.Remove(dictionary);

        await _context.SaveChangesAsync(cancellationToken);
    }
}