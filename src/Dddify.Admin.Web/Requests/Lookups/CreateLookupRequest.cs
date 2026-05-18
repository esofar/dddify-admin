namespace Dddify.Admin.Web.Requests.Lookups;

/// <summary>
/// 新增字典请求。
/// </summary>
/// <param name="Code">字典编码。</param>
/// <param name="Name">字典名称。</param>
/// <param name="Description">字典描述。</param>
public sealed record CreateLookupRequest(
    string Code,
    string Name,
    string? Description);
