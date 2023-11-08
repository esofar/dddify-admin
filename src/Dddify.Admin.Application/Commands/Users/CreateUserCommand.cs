using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record CreateUserCommand(
        string FullName,
        string? NickName,
        string Gender,
        DateTime? BirthDate,
        string Email,
        string PhoneNumber,
        Guid OrganizationId,
        string Type) : ICommand;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.FullName).NotEmpty().MaximumLength(20);
        RuleFor(c => c.NickName).MaximumLength(20);
        RuleFor(c => c.Gender).NotNull().IsEnumName(typeof(UserGender));
        RuleFor(c => c.Email).NotEmpty().MaximumLength(100).EmailAddress();
        RuleFor(c => c.PhoneNumber).NotEmpty().Matches(RegexStrings.PhoneNumber);
        RuleFor(c => c.OrganizationId).NotEmpty();
        RuleFor(c => c.Type).NotEmpty();
    }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHelper _passwordHelper;

    public CreateUserCommandHandler(IApplicationDbContext context, IPasswordHelper passwordHelper)
    {
        _context = context;
        _passwordHelper = passwordHelper;
    }

    public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(c => c.Email == command.Email, cancellationToken))
        {
            throw new UserEmailDuplicateException(command.Email);
        }

        if (await _context.Users.AnyAsync(c => c.PhoneNumber == command.PhoneNumber, cancellationToken))
        {
            throw new UserPhoneNumberDuplicateException(command.PhoneNumber);
        }

        var passwordHash = _passwordHelper.Hash("Password@2023");

        var user = new User(
            command.FullName,
            command.NickName,
            command.Gender.ToEnum<UserGender>(),
            command.BirthDate.HasValue ? DateOnly.FromDateTime(command.BirthDate.Value) : default,
            command.Email,
            command.PhoneNumber,
            command.OrganizationId,
            command.Type,
            passwordHash);

        await _context.Users.AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}