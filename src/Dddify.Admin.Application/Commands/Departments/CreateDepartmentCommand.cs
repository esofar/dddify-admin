using Dddify.Admin.Application.Exceptions.Departments;
using Dddify.Admin.Domain.Aggregates.Departments;
using Dddify.Admin.Domain.Exceptions.Departments;

namespace Dddify.Admin.Application.Commands.Departments;

public record CreateDepartmentCommand(
    Guid? ParentId,
    string Name,
    string Type,
    Guid LeaderId,
    string LeaderName,
    bool IsEnabled,
    int Order) : ICommand;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Department.MaxNameLength);

        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(Department.MaxTypeLength);

        RuleFor(x => x.LeaderId)
            .NotEmpty();

        RuleFor(x => x.LeaderName)
            .NotEmpty()
            .MaximumLength(DepartmentLeader.MaxNameLength);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);
    }
}

public class CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IGuidGenerator guidGenerator, IShortCodeGenerator shortCodeGenerator) : ICommandHandler<CreateDepartmentCommand>
{
    public async Task Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        if (!await departmentRepository.IsNameUniqueAsync(command.Name, command.ParentId))
        {
            throw new DepartmentNameAlreadyExistsException(command.Name);
        }

        var id = guidGenerator.Create();
        var code = shortCodeGenerator.Generate(id);

        Department? department;
        if (command.ParentId.HasValue)
        {
            var parentDepartment = await departmentRepository.GetAsync(command.ParentId.Value, cancellationToken)
                ?? throw new DepartmentNotFoundException(command.ParentId.Value);

            department = parentDepartment.CreateChild(
                id,
                command.Name,
                code,
                command.Type,
                command.LeaderId,
                command.LeaderName,
                command.IsEnabled,
                command.Order);
        }
        else
        {
            department = Department.CreateRoot(
                id,
                command.Name,
                code,
                command.Type,
                command.LeaderId,
                command.LeaderName,
                command.IsEnabled,
                command.Order);
        }

        await departmentRepository.AddAsync(department, cancellationToken);
    }
}
