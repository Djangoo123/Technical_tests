using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MailCrawler.Core;

public static class HtmlLinkExtractor
{
    private static readonly Regex WinDrivePathRegex = new(@"^[a-zA-Z]:[\\/]", RegexOptions.Compiled);

    public static Uri ToAbsoluteUri(string urlOrPath)
    {
        if (string.IsNullOrWhiteSpace(urlOrPath))
            throw new ArgumentException("startUrl is required.", nameof(urlOrPath));

        var s = urlOrPath.Trim();

        if (Uri.TryCreate(s, UriKind.Absolute, out var abs) &&
            (abs.Scheme is "http" or "https" or "file"))
        {
            return abs;
        }

        if (LooksLikeWindowsPath(s))
            return WindowsPathToFileUri(s);


        if (Uri.TryCreate(s, UriKind.Relative, out var rel))
        {
            var combined = new Uri(WindowsPathToFileUri(Environment.CurrentDirectory + "\\"), rel);
            return combined;
        }

        throw new ArgumentException("startUrl must be an absolute http(s) URL or a file path.", nameof(urlOrPath));
    }

    private static bool LooksLikeWindowsPath(string s)
    {
        // path : C:\... ou C:/...
        if (WinDrivePathRegex.IsMatch(s)) return true;

        // UNC : \\server\share\file.html
        if (s.StartsWith(@"\\", StringComparison.Ordinal)) return true;

        return false;
    }

    private static Uri WindowsPathToFileUri(string windowsPath)
    {
        var p = windowsPath.Trim().Replace('\\', '/');

        // UNC: \\server\share\file.html  =>  file://server/share/file.html
        if (p.StartsWith("//", StringComparison.Ordinal))
        {
            var without = p.Substring(2);
            var firstSlash = without.IndexOf('/');
            if (firstSlash <= 0) throw new ArgumentException("Invalid UNC path.", nameof(windowsPath));

            var host = without.Substring(0, firstSlash);
            var rest = without.Substring(firstSlash); // begins with '/'
            return new Uri($"file://{host}{rest}");
        }

        //  path: C:/TestHtml/index.html => file:///C:/TestHtml/index.html
        p = p.TrimStart('/'); // in case some dingus passes /C:/...
        return new Uri($"file:///{p}");
    }

    public sealed record Extraction(IReadOnlyCollection<string> Emails, IReadOnlyCollection<Uri> Links);

    public static Extraction Extract(Uri baseUri, string html)
    {
        if (baseUri == null) throw new ArgumentNullException(nameof(baseUri));
        if (html == null) throw new ArgumentNullException(nameof(html));

        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var links = new HashSet<Uri>();

        XDocument doc;
        try
        {
            // Case: HTML is valid XML.
            doc = XDocument.Parse(html, LoadOptions.None);
        }
        catch
        {
            return new Extraction([], []);
        }

        foreach (var a in doc.Descendants("a"))
        {
            var href = a.Attribute("href")?.Value?.Trim();
            if (string.IsNullOrWhiteSpace(href))
                continue;

            if (href.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var email in ParseMailto(href))
                    emails.Add(email);

                continue;
            }

            // ignore anchors / javascript pseudo-links
            if (href.StartsWith("#") ||
                href.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                continue;

            Uri resolved;
            try
            {
                resolved = new Uri(baseUri, href);
            }
            catch
            {
                continue;
            }

            // Only keep http(s) or file
            if (resolved.Scheme is not ("http" or "https" or "file"))
                continue;

            links.Add(resolved);
        }

        return new Extraction([.. emails], [.. links]);
    }

    public static IEnumerable<string> ParseMailto(string href)
    {
        // mailto:user@x.tld?subject=Hello
        var afterScheme = href["mailto:".Length..];

        var addressPart = afterScheme.Split('?', 2)[0];

        var candidates = addressPart.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var c in candidates)
        {
            var email = Uri.UnescapeDataString(c).Trim();

            if (email.Length == 0) continue;
            if (!email.Contains('@')) continue;
            if (email.Any(char.IsWhiteSpace)) continue;

            yield return email;
        }
    }

    public static string NormalizePageIdentity(Uri uri)
    {
        // remove fragment (#...) so it doesn't create duplicates.
        var noFragment = new UriBuilder(uri) { Fragment = "" }.Uri;
        return noFragment.ToString();
    }
}
