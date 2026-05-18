namespace Dddify.Admin.Application.Dtos.Lookups;

/// <summary>
/// 可用字典项。
/// </summary>
/// <param name="Value">字典项值。</param>
/// <param name="Label">字典项标签。</param>
/// <param name="Color">显示颜色。</param>
public record LookupActiveItemDto(string Value, string Label, string Color);
