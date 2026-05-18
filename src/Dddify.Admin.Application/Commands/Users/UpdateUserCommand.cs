using Dddify.Admin.Application.Exceptions;
using Dddify.Admin.Application.Exceptions.Users;
using Dddify.Admin.Domain.Aggregates.Users;

namespace Dddify.Admin.Application.Commands.Users;

public record UpdateUserCommand(
    Guid Id,
    string Name,
    string? NickName,
    string Gender,
    DateOnly? BirthDate,
    string Email,
    string PhoneNumber,
    Guid DepartmentId,
    string DepartmentName) : ICommand;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(User.MaxNameLength)
            .Matches(User.NamePattern);

        RuleFor(x => x.NickName)
            .MaximumLength(User.MaxNickNameLength)
            .Matches(User.NickNamePattern);

        RuleFor(x => x.Gender)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(20)
            .IsEnumName(typeof(UserGender), caseSensitive: false);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
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
    }
}

public class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new UserNotFoundException(command.Id);

        if (!await userRepository.IsEmailUniqueAsync(command.Email, user.Id))
        {
            throw new UserEmailDuplicateException(command.Email);
        }

        if (!await userRepository.IsPhoneNumberUniqueAsync(command.PhoneNumber, user.Id))
        {
            throw new UserPhoneNumberDuplicateException(command.PhoneNumber);
        }

        var gender = Enum.Parse<UserGender>(command.Gender.Trim(), ignoreCase: true);

        user.Change(
            command.Name,
            command.NickName,
            gender,
            command.BirthDate,
            command.Email,
            command.PhoneNumber);

        user.ChangeDepartment(command.DepartmentId, command.DepartmentName);
    }
}
