using Dddify.Admin.Application.Dtos.Departments;

namespace Dddify.Admin.Application.Queries.Departments;

public record GetAllDepartmentsQuery : IQuery<IEnumerable<DepartmentListDto>>;

public class GetAllDepartmentsQueryHandler(IDepartmentRepository departmentRepository) : IQueryHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentListDto>>
{
    public async Task<IEnumerable<DepartmentListDto>> Handle(GetAllDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var departments = await departmentRepository.GetAllAsync(cancellationToken);

        return departments.Adapt<IEnumerable<DepartmentListDto>>();
    }
}