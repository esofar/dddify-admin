using Dddify.Admin.Domain.Aggregates.Announcements;

namespace Dddify.Admin.Application.Abstractions.Announcements;

/// <summary>
/// 公告内容规范化服务。
/// </summary>
public interface IContentNormalizer
{
    /// <summary>
    /// 清理 HTML 并提取纯文本。
    /// </summary>
    /// <param name="html">原始 HTML 内容。</param>
    /// <returns>清理后的 HTML 及纯文本。</returns>
    AnnouncementContent Normalize(string html);
}
