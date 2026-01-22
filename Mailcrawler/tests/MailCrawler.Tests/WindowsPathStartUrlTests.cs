using MailCrawler.Core;
using Xunit;

namespace MailCrawler.Tests;

public class WindowsPathStartUrlTests
{
    [Fact]
    public void Accepts_SlashPath()
    {
        var u = HtmlLinkExtractor.ToAbsoluteUri("C:/TestHtml/index.html");
        Assert.Equal("file", u.Scheme);
        Assert.EndsWith("C:/TestHtml/index.html", u.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Accepts_BackslashPath()
    {
        var u = HtmlLinkExtractor.ToAbsoluteUri(@"C:\TestHtml\index.html");
        Assert.Equal("file", u.Scheme);
        Assert.EndsWith("C:/TestHtml/index.html", u.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Crawl_Works_With_WinPath()
    {
        var start = "C:/TestHtml/index.html";
        var baseUri = HtmlLinkExtractor.ToAbsoluteUri(start);

        const string p0 = """
        <html>
          <a href="./child1.html">child1</a>
          <a href="mailto:nullepart@mozilla.org">n</a>
        </html>
        """;

        const string p1 = """
        <html>
          <a href="./child2.html">child2</a>
          <a href="mailto:ailleurs@mozilla.org">a</a>
        </html>
        """;

        const string p2 = """
        <html>
          <a href="mailto:loin@mozilla.org">l</a>
        </html>
        """;

        var site = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [baseUri.ToString()] = p0,
            [new Uri(baseUri, "./child1.html").ToString()] = p1,
            [new Uri(baseUri, "./child2.html").ToString()] = p2,
        };

        var sut = new EmailCrawler(new FakeBrowser(site));
        var res = sut.GetEmailsInPageAndChildPages(start, 2);

        Assert.Equal(3, res.Count);
        Assert.Contains("nullepart@mozilla.org", res);
        Assert.Contains("ailleurs@mozilla.org", res);
        Assert.Contains("loin@mozilla.org", res);
    }
}
