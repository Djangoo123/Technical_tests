using Microsoft.Extensions.DependencyInjection;
using TechnicalTestCS.Api.Interfaces;
using TechnicalTestCS.Api.Services;
using TechnicalTestCS.Domain;
using TechnicalTestCS.Infrastructure.Persistence;

namespace TechnicalTestCS.Tests.ServiceTests;

public class InvalidTransitionTests
{
    [Fact]
    public async Task Invalid_transition_is_rejected()
    {
        FakeExternalArticlesClient? external = new FakeExternalArticlesClient(
            FakeExternalArticlesClient.Article(1, "A1")
        );

        using var host = new TestHost(external);
        using var scope = host.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.ArticleStatuses.Add(new ArticleStatusEntity
        {
            ArticleId = 1,
            Status = ArticleStatus.Pending,
            UpdatedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var statusSvc = scope.ServiceProvider.GetRequiredService<IArticleStatusService>();

        await Assert.ThrowsAsync<InvalidTransitionException>(() =>
            statusSvc.UpdateStatus(1, ArticleStatus.Draft, default));
    }

}
