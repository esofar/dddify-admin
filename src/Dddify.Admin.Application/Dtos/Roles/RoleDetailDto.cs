namespace Dddify.Admin.Application.Dtos.Roles;

public record RoleDetailDto: RoleDto
{
    public string ConcurrencyStamp { get; set; }
}