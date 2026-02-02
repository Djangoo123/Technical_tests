using TechnicalTestCS.Infrastructure.External.Dtos;
using TechnicalTestCS.Infrastructure.External.Interfaces;

namespace TechnicalTestCS.Tests;

public sealed class FakeExternalArticlesClient : IExternalArticlesClient
{
    private readonly List<ArticleExternalDto> _items;

    public FakeExternalArticlesClient(params ArticleExternalDto[] items)
    {
        _items = [.. items];
    }

    public Task<IReadOnlyList<ArticleExternalDto>> GetArticles(CancellationToken ct)
        => Task.FromResult((IReadOnlyList<ArticleExternalDto>)_items);

    public Task<ArticleExternalDto?> GetArticle(int id, CancellationToken ct)
        => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public static ArticleExternalDto Article(int id, string title, string slug = "")
        => new()
        {
            Id = id,
            Title = title,
            Slug = string.IsNullOrWhiteSpace(slug) ? $"a-{id}" : slug
        };
}
