namespace Dddify.Admin.Application.Dtos.Organizations;

public record OrganizationDetailDto : OrganizationDto
{
    public string ConcurrencyStamp { get; set; }
}