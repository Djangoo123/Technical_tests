using Microsoft.Extensions.DependencyInjection;
using TechnicalTestCS.Api.Interfaces;
using Xunit;

namespace TechnicalTestCS.Tests.ServiceTests;

public class ArticlesService_QueryFilterTests
{
    [Fact]
    public async Task GetArticles_filters_by_q_on_title_or_slug()
    {
        var external = new FakeExternalArticlesClient(
            FakeExternalArticlesClient.Article(1, "Hello World", "hello-world"),
            FakeExternalArticlesClient.Article(2, "Another", "something")
        );

        using var host = new TestHost(external);
        using var scope = host.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IArticlesService>();

        var items = await svc.GetArticles(q: "hello", status: null, ct: default);

        Assert.Single(items);
        Assert.Equal(1, items[0].Id);
    }
}
