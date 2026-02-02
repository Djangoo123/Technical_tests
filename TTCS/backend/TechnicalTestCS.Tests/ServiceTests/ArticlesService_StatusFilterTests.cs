using Microsoft.Extensions.DependencyInjection;
using TechnicalTestCS.Api.Interfaces;
using Xunit;

namespace TechnicalTestCS.Tests.ServiceTests;

public class ArticlesService_StatusFilterTests
{
    [Fact]
    public async Task GetArticles_throws_on_unknown_status_filter()
    {
        FakeExternalArticlesClient external = new(
            FakeExternalArticlesClient.Article(1, "A1")
        );

        using var host = new TestHost(external);
        using var scope = host.CreateScope();

        var svc = scope.ServiceProvider.GetRequiredService<IArticlesService>();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.GetArticles(q: null, status: "not-a-status", ct: default));
    }
}
