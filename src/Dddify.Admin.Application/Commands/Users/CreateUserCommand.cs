using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record CreateUserCommand(
    string Password,
    string Name,
    string? NickName,
    string Gender,
    DateOnly? BirthDate,
    string Email,
    string PhoneNumber,
    Guid DepartmentId,
    string DepartmentName,
    IEnumerable<RoleEntry> DefaultRoles) : ICommand;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(User.MaxPasswordLength)
            .Matches(User.PasswordPattern);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(User.MaxNameLength)
            .Matches(User.NamePattern);

        RuleFor(x => x.NickName)
            .MaximumLength(User.MaxNickNameLength)
            .Matches(User.NickNamePattern);

        RuleFor(x => x.Gender)
            .NotEmpty()
            .MaximumLength(20)
            .IsEnumName(typeof(UserGender), caseSensitive: false);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(User.MaxEmailLength)
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(User.MaxPhoneNumberLength);

        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.DepartmentName)
            .NotEmpty()
            .MaximumLength(UserDepartment.MaxNameLength);

        RuleFor(x => x.DefaultRoles)
            .NotEmpty();

        RuleForEach(x => x.DefaultRoles)
            .NotNull()
            .ChildRules(role =>
            {
                role.RuleFor(x => x.RoleId)
                    .NotEmpty();

                role.RuleFor(x => x.RoleName)
                    .NotEmpty()
                    .MaximumLength(UserRole.MaxRoleNameLength);
            });
    }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IGuidGenerator guidGenerator,
    IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (!await userRepository.IsEmailUniqueAsync(command.Email))
        {
            throw new UserEmailDuplicateException(command.Email);
        }

        if (!await userRepository.IsPhoneNumberUniqueAsync(command.PhoneNumber))
        {
            throw new UserPhoneNumberDuplicateException(command.PhoneNumber);
        }

        var userId = guidGenerator.Create();
        var passwordHash = passwordHasher.Hash(command.Password);
        var gender = Enum.Parse<UserGender>(command.Gender.Trim(), ignoreCase: true);

        var userRoles = command.DefaultRoles.Select(c => new UserRole(userId, c.RoleId, c.RoleName));

        var user = new User(
            userId,
            passwordHash,
            command.Name,
            command.NickName,
            gender,
            command.BirthDate,
            command.Email,
            command.PhoneNumber,
            command.DepartmentId,
            command.DepartmentName);

        user.AssignRoles(userRoles);

        await userRepository.AddAsync(user, cancellationToken);
    }
}
