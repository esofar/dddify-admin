using Dddify.Admin.Domain.Exceptions.Dictionaries;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record UpdateDictionaryCommand(
    Guid Id,
    string Name,
    string? Description,
    string ConcurrencyStamp) : ICommand;

public class UpdateDictionaryCommandValidator : AbstractValidator<UpdateDictionaryCommand>
{
    public UpdateDictionaryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Description).MaximumLength(50);
        RuleFor(c => c.ConcurrencyStamp).NotEmpty();
    }
}

public class UpdateDictionaryCommandHandler : ICommandHandler<UpdateDictionaryCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDictionaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateDictionaryCommand command, CancellationToken cancellationToken)
    {
        var dictionary = await _context.Dictionaries.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (dictionary is null)
        {
            throw new DictionaryNotFoundException(command.Id);
        }

        if (await _context.Dictionaries.AnyAsync(c => c.Id != command.Id && c.Name == command.Name, cancellationToken))
        {
            throw new DictionaryNameDuplicateException(command.Name);
        }

        _context.ResetConcurrencyStamp(dictionary, command.ConcurrencyStamp);

        dictionary.Update(command.Name, command.Description);

        await _context.SaveChangesAsync(cancellationToken);
    }
}