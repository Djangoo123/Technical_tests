using MailCrawler.Core;
using Xunit;

namespace MailCrawler.Tests;

public class EmailCrawlerTests
{
    private const string P0 = """
    <html>
      <h1>INDEX</h1>
      <a href="./child1.html">child1</a>
      <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a>
    </html>
    """;

    private const string P1 = """
    <html>
      <h1>CHILD1</h1>
      <a href="./index.html">index</a>
      <a href="./child2.html">child2</a>
      <a href="mailto:ailleurs@mozilla.org">Envoyer l'email ailleurs</a>
      <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a>
    </html>
    """;

    private const string P2 = """
    <html>
      <h1>CHILD2</h1>
      <a href="./index.html#top">index</a>
      <a href="mailto:loin@mozilla.org?subject=Hello">Envoyer l'email loin</a>
      <a href="mailto:nullepart@mozilla.org">Envoyer l'email nulle part</a>
    </html>
    """;

    private static (EmailCrawler sut, string start) Sut()
    {
        var start = "file:///C:/TestHtml/index.html";
        var baseUri = new Uri(start);

        var site = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [baseUri.ToString()] = P0,
            [new Uri(baseUri, "./child1.html").ToString()] = P1,
            [new Uri(baseUri, "./child2.html").ToString()] = P2,
        };

        return (new EmailCrawler(new FakeBrowser(site)), start);
    }

    [Fact]
    public void Depth0()
    {
        var (sut, start) = Sut();
        var res = sut.GetEmailsInPageAndChildPages(start, 0);

        Assert.Single(res);
        Assert.Contains("nullepart@mozilla.org", res);
    }

    [Fact]
    public void Depth1()
    {
        var (sut, start) = Sut();
        var res = sut.GetEmailsInPageAndChildPages(start, 1);

        Assert.Equal(2, res.Count);
        Assert.Contains("nullepart@mozilla.org", res);
        Assert.Contains("ailleurs@mozilla.org", res);
    }

    [Fact]
    public void Depth2()
    {
        var (sut, start) = Sut();
        var res = sut.GetEmailsInPageAndChildPages(start, 2);

        Assert.Equal(3, res.Count);
        Assert.Contains("loin@mozilla.org", res);
    }

    [Fact]
    public void Depth_All_Works()
    {
        var (sut, start) = Sut();
        var res = sut.GetEmailsInPageAndChildPages(start, -1);

        Assert.Equal(3, res.Count);
    }

    [Fact]
    public void No_Loop()
    {
        var (sut, start) = Sut();
        var res = sut.GetEmailsInPageAndChildPages(start, -1);

        Assert.Contains("nullepart@mozilla.org", res);
    }

    [Fact]
    public void MailtoQuery_Ignored()
    {
        var (sut, start) = Sut();
        var res = sut.GetEmailsInPageAndChildPages(start, 2);

        Assert.Contains("loin@mozilla.org", res);
    }
}
