using MailCrawler.Core;

namespace MailCrawler.Tests;

public sealed class FakeBrowser : IBrowser
{
    private readonly Dictionary<string, string> _pages;

    public FakeBrowser(Dictionary<string, string> pages)
    {
        _pages = pages ?? throw new ArgumentNullException(nameof(pages));
    }

    public string GetHtml(string url)
    {
        // Normalize fragment
        var key = new UriBuilder(new Uri(url)) { Fragment = "" }.Uri.ToString();

        if (_pages.TryGetValue(key, out var html))
            return html;

        throw new FileNotFoundException($"Page not found in FakeBrowser: {key}");
    }
}
