namespace Dddify.Admin.Web.Requests.Lookups;

/// <summary>
/// 修改字典请求。
/// </summary>
/// <param name="Name">字典名称。</param>
/// <param name="Description">字典描述。</param>
public sealed record UpdateLookupRequest(
    string Name,
    string? Description);
