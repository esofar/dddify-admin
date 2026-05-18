namespace Dddify.Admin.Web.Requests.Lookups;

/// <summary>
/// 修改字典项请求。
/// </summary>
/// <param name="Label">字典项标签。</param>
/// <param name="Color">显示颜色。</param>
public sealed record UpdateLookupItemRequest(
    string Label,
    string? Color);
