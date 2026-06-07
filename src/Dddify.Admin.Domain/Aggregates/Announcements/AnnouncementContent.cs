namespace Dddify.Admin.Domain.Aggregates.Announcements;

public sealed class AnnouncementContent : ValueObject
{
    public const int MaxHtmlLength = 100000;
    public const int MaxPlainTextLength = 100000;

    public string Html { get; private set; } = default!;

    public string PlainText { get; private set; } = default!;

    private AnnouncementContent() { }

    public AnnouncementContent(string html, string plainText)
    {
        Html = html;
        PlainText = plainText;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Html;
        yield return PlainText;
    }
}
