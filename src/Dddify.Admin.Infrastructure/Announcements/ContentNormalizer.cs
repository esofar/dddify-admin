using Dddify.Admin.Application.Services.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;
using Ganss.Xss;
using HtmlAgilityPack;
using System.Net;

namespace Dddify.Admin.Infrastructure.Announcements;

[ScopedDependency(RegistrationMode.AsImplementedInterfaces)]
public class ContentNormalizer() : IContentNormalizer
{
    public AnnouncementContent Normalize(string html)
    {
        var safeHtml = Sanitize(html);
        var plainText = ToPlainText(safeHtml);

        return new AnnouncementContent(safeHtml, plainText);
    }

    private static string Sanitize(string html)
    {
        var htmlSanitizer = new HtmlSanitizer();

        return htmlSanitizer.Sanitize(html);
    }

    private static string ToPlainText(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
    }
}
