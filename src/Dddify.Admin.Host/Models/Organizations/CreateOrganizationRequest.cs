namespace Dddify.Admin.Application.Dtos.Organizations;

public record CreateOrganizationRequest
{
    /// <summary>
    /// 上级机构ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 机构类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 排序码
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}