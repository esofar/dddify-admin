namespace Dddify.Admin.Application.Dtos.Roles;

public record RoleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}