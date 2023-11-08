namespace Dddify.Admin.Application.Dtos.Organizations;

public record UpdateOrganizationRequest : CreateOrganizationRequest
{
    /// <summary>
    /// 并发戳
    /// </summary>
    public string ConcurrencyStamp { get; set; }
}