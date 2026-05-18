namespace Dddify.Admin.Web.Requests.Lookups;

/// <summary>
/// 新增字典项请求。
/// </summary>
/// <param name="Value">字典项值。</param>
/// <param name="Label">字典项标签。</param>
/// <param name="Color">显示颜色。</param>
public sealed record CreateLookupItemRequest(
    string Value,
    string Label,
    string? Color);
