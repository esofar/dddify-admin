namespace Dddify.Admin.Web.Requests.Lookups;

/// <summary>
/// 查询字典列表请求。
/// </summary>
/// <param name="Current">当前页码。</param>
/// <param name="PageSize">每页数量。</param>
/// <param name="Code">字典编码。</param>
/// <param name="Name">字典名称。</param>
public sealed record SearchLookupRequest(
    int Current,
    int PageSize,
    string? Code,
    string? Name);
