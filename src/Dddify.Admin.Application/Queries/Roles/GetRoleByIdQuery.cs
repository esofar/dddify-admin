using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Exceptions.Roles;

namespace Dddify.Admin.Application.Queries.Roles;

public record GetRoleByIdQuery(Guid Id) : IQuery<RoleDetailDto>;

public class GetRoleByIdQueryHandler(IRoleRepository roleRepository) : IQueryHandler<GetRoleByIdQuery, RoleDetailDto>
{
    public async Task<RoleDetailDto> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await roleRepository.GetAsync(query.Id, cancellationToken)
            ?? throw new RoleNotFoundException(query.Id);

        return user.Adapt<RoleDetailDto>();
    }
}