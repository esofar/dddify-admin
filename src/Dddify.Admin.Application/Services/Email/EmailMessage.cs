namespace Dddify.Admin.Application.Services.Email;

public sealed record EmailMessage(string To, string Subject, string Body)
{
    public bool IsHtml { get; init; } = true;

    public string? From { get; init; }

    public IReadOnlyList<string> Cc { get; init; } = [];

    public IReadOnlyList<string> Bcc { get; init; } = [];
}
