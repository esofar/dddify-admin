namespace Dddify.Admin.Application.Dtos.Organizations;

public record OrganizationDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
}