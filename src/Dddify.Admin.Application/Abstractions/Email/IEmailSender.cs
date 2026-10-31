namespace Dddify.Admin.Application.Abstractions.Email;

/// <summary>
/// 邮件发送服务。
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// 发送邮件。
    /// </summary>
    /// <param name="message">邮件内容。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// 待发送的邮件。
/// </summary>
/// <param name="To">收件人邮箱。</param>
/// <param name="Subject">邮件主题。</param>
/// <param name="Body">邮件正文。</param>
public sealed record EmailMessage(string To, string Subject, string Body)
{
    /// <summary>
    /// 是否使用 HTML 正文，默认为 <see langword="true"/>。
    /// </summary>
    public bool IsHtml { get; init; } = true;

    /// <summary>
    /// 发件人邮箱，未指定时使用默认配置。
    /// </summary>
    public string? From { get; init; }

    /// <summary>
    /// 抄送邮箱列表。
    /// </summary>
    public IReadOnlyList<string> Cc { get; init; } = [];

    /// <summary>
    /// 密送邮箱列表。
    /// </summary>
    public IReadOnlyList<string> Bcc { get; init; } = [];
}
