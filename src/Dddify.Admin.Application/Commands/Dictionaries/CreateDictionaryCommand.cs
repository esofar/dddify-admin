using Dddify.Admin.Domain.Exceptions.Dictionaries;
using Microsoft.Extensions.Caching.Distributed;

namespace Dddify.Admin.Application.Commands.Dictionaries;

public record CreateDictionaryCommand(
    string Code,
    string Name,
    string? Description) : ICommand;

public class CreateDictionaryCommandValidator : AbstractValidator<CreateDictionaryCommand>
{
    public CreateDictionaryCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Description).MaximumLength(50);
    }
}

public class CreateDictionaryCommandHandler : ICommandHandler<CreateDictionaryCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDictionaryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateDictionaryCommand command, CancellationToken cancellationToken)
    {
        if (await _context.Dictionaries.AnyAsync(c => c.Code == command.Code, cancellationToken))
        {
            throw new DictionaryCodeDuplicateException(command.Code);
        }

        if (await _context.Dictionaries.AnyAsync(c => c.Name == command.Name, cancellationToken))
        {
            throw new DictionaryNameDuplicateException(command.Name);
        }

        var dictionary = new Dictionary(command.Code, command.Name, command.Description);

        await _context.Dictionaries.AddAsync(dictionary, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}