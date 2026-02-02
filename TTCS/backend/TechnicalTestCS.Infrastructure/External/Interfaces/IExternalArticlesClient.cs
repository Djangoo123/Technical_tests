
using TechnicalTestCS.Infrastructure.External.Dtos;

namespace TechnicalTestCS.Infrastructure.External.Interfaces
{
    public interface IExternalArticlesClient
    {
        Task<IReadOnlyList<ArticleExternalDto>> GetArticles(CancellationToken ct);
        Task<ArticleExternalDto?> GetArticle(int id, CancellationToken ct);
    }
}
