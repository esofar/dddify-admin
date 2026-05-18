using Dddify.Admin.Application.Exceptions.Departments;
using Dddify.Admin.Domain.Aggregates.Departments;
using Dddify.Admin.Domain.Exceptions.Departments;

namespace Dddify.Admin.Application.Commands.Departments;

public record UpdateDepartmentCommand(
    Guid Id,
    Guid? ParentId,
    string Name,
    string Type,
    Guid LeaderId,
    string LeaderName,
    bool IsEnabled,
    int Order,
    string? ConcurrencyStamp) : ICommand;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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

public class UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository) : ICommandHandler<UpdateDepartmentCommand>
{
    public async Task Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new DepartmentNotFoundException(command.Id);

        if (!await departmentRepository.IsNameUniqueAsync(command.Name, command.ParentId, command.Id))
        {
            throw new DepartmentNameAlreadyExistsException(command.Name);
        }

        if (command.ParentId.HasValue)
        {
            var parentDepartment = await departmentRepository.GetAsync(command.ParentId.Value, cancellationToken)
                ?? throw new DepartmentNotFoundException(command.ParentId.Value);

            if (parentDepartment.Path.StartsWith(department.Path))
            {
                throw new DepartmentParentCycleException(command.Id, command.ParentId.Value);
            }

            department.MoveTo(parentDepartment.Id, parentDepartment.Path, parentDepartment.FullName, parentDepartment.Level);
        }
        else
        {
            department.MoveTo(null, string.Empty, null, -1);
        }

        department.ChangeName(command.Name);
        department.ChangeLeader(command.LeaderId, command.LeaderName);
        department.ChangeOrder(command.Order);

        if (command.IsEnabled)
        {
            department.Enable();
        }
        else
        {
            department.Disable();
        }

        departmentRepository.SetOriginalConcurrencyStamp(department, command.ConcurrencyStamp);
    }
}
