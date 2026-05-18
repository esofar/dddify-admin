using Dddify.Admin.Application.Dtos.Departments;
using Dddify.Admin.Application.Exceptions.Departments;

namespace Dddify.Admin.Application.Queries.Departments;

public record GetDepartmentByIdQuery(Guid Id) : IQuery<DepartmentDetailDto>;

public class GetDepartmentByIdQueryHandler(IDepartmentRepository departmentRepository) : IQueryHandler<GetDepartmentByIdQuery, DepartmentDetailDto>
{
    public async Task<DepartmentDetailDto> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetAsync(query.Id, cancellationToken)
            ?? throw new DepartmentNotFoundException(query.Id);

        return department.Adapt<DepartmentDetailDto>();
    }
}