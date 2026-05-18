namespace Dddify.Admin.Application.Dtos.Lookups;

/// <summary>
/// 字典项。
/// </summary>
/// <param name="Id">字典项 ID。</param>
/// <param name="Value">字典项值。</param>
/// <param name="Label">字典项标签。</param>
/// <param name="Color">显示颜色。</param>
/// <param name="Order">排序值。</param>
/// <param name="IsPreset">是否为预置项。</param>
/// <param name="IsEnabled">是否启用。</param>
public record LookupItemDto(Guid Id, string Value, string Label, string? Color, int Order, bool IsPreset, bool IsEnabled);
