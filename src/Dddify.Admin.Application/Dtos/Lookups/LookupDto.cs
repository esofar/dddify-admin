namespace Dddify.Admin.Application.Dtos.Lookups;

/// <summary>
/// 字典。
/// </summary>
/// <param name="Id">字典 ID。</param>
/// <param name="Code">字典编码。</param>
/// <param name="Name">字典名称。</param>
/// <param name="Description">描述。</param>
public record LookupDto(Guid Id, string Code, string Name, string? Description);
