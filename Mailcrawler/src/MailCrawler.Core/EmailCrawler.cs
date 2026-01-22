using System.Collections.Concurrent;

namespace MailCrawler.Core;

public sealed class EmailCrawler
{
    private readonly IBrowser _browser;

    public EmailCrawler(IBrowser browser)
    {
        _browser = browser ?? throw new ArgumentNullException(nameof(browser));
    }

    /// <summary>
    /// BFS crawl: explore closest pages first (depth 0, then 1 etc).
    /// maximumDepth = -1 => explore all reachable pages.
    /// </summary>
    public IReadOnlyCollection<string> GetEmailsInPageAndChildPages(string startUrl, int maximumDepth)
    {
        if (string.IsNullOrWhiteSpace(startUrl))
            throw new ArgumentException("startUrl is required.", nameof(startUrl));

        if (maximumDepth < -1)
            throw new ArgumentOutOfRangeException(nameof(maximumDepth), "maximumDepth must be -1 or >= 0.");

        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var visitedPages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var startUri = HtmlLinkExtractor.ToAbsoluteUri(startUrl);

        var queue = new Queue<(Uri uri, int depth)>();
        queue.Enqueue((startUri, 0));

        while (queue.Count > 0)
        {
            var (currentUri, depth) = queue.Dequeue();
            var currentKey = HtmlLinkExtractor.NormalizePageIdentity(currentUri);

            if (!visitedPages.Add(currentKey))
                continue;

            string html;
            try
            {
                html = _browser.GetHtml(currentUri.ToString());
            }
            catch
            {
                continue;
            }

            var extraction = HtmlLinkExtractor.Extract(currentUri, html);

            foreach (var email in extraction.Emails)
                emails.Add(email);

            var canGoDeeper = maximumDepth == -1 || depth < maximumDepth;
            if (!canGoDeeper)
                continue;

            foreach (var link in extraction.Links)
            {
                var linkKey = HtmlLinkExtractor.NormalizePageIdentity(link);
                if (!visitedPages.Contains(linkKey))
                {
                    queue.Enqueue((link, depth + 1));
                }
            }
        }

        return [.. emails];
    }
}
