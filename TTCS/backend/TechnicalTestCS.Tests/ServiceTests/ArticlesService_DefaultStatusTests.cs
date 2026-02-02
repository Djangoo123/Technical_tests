using Microsoft.Extensions.DependencyInjection;
using TechnicalTestCS.Api.Interfaces;
using Xunit;

namespace TechnicalTestCS.Tests.ServiceTests;

public class ArticlesService_DefaultStatusTests
{
    [Fact]
    public async Task GetArticles_defaults_to_draft_when_not_in_db()
    {
        var external = new FakeExternalArticlesClient(
            FakeExternalArticlesClient.Article(1, "A1"),
            FakeExternalArticlesClient.Article(2, "A2")
        );

        using var host = new TestHost(external);
        using var scope = host.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IArticlesService>();

        var items = await svc.GetArticles(q: null, status: null, ct: default);

        Assert.Equal(2, items.Count);
        Assert.All(items, x => Assert.Equal("draft", x.Status));
    }
}
