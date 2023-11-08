using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record UpdateUserCommand(
    Guid Id,
    string FullName,
    string? NickName,
    string Gender,
    DateTime? BirthDate,
    string Email,
    string PhoneNumber,
    Guid OrganizationId,
    string Type,
    string ConcurrencyStamp) : ICommand;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.FullName).NotEmpty().MaximumLength(20);
        RuleFor(c => c.NickName).MaximumLength(20);
        RuleFor(c => c.Gender).NotNull().IsEnumName(typeof(UserGender));
        RuleFor(c => c.Email).NotEmpty().MaximumLength(100).EmailAddress();
        RuleFor(c => c.PhoneNumber).NotEmpty().Matches(RegexStrings.PhoneNumber);
        RuleFor(c => c.OrganizationId).NotEmpty();
        RuleFor(c => c.Type).NotEmpty();
        RuleFor(c => c.ConcurrencyStamp).NotEmpty();
    }
}

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(command.Id);
        }

        if (await _context.Users.AnyAsync(c => c.Id != command.Id && c.Email == command.Email, cancellationToken))
        {
            throw new UserEmailDuplicateException(command.Email);
        }

        if (await _context.Users.AnyAsync(c => c.Id != command.Id && c.PhoneNumber == command.PhoneNumber, cancellationToken))
        {
            throw new UserPhoneNumberDuplicateException(command.PhoneNumber);
        }

        _context.ResetConcurrencyStamp(user, command.ConcurrencyStamp);

        user.Update(
            command.FullName,
            command.NickName,
            command.Gender.ToEnum<UserGender>(),
            command.BirthDate.HasValue ? DateOnly.FromDateTime(command.BirthDate.Value) : default,
            command.Email,
            command.PhoneNumber,
            command.OrganizationId,
            command.Type);

        await _context.SaveChangesAsync(cancellationToken);
    }
}