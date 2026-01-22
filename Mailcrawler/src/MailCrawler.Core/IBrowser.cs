namespace MailCrawler.Core;

public interface IBrowser
{
    string GetHtml(string url);
}
