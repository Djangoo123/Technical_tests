using TechnicalTestCS.Api.Dtos;

namespace TechnicalTestCS.Api.Interfaces
{
    public interface IArticlesService
    {
        Task<IReadOnlyList<ArticleSummaryDto>> GetArticles(string? q, string? status, CancellationToken ct);
        Task<ArticleDetailDto?> GetArticle(int id, CancellationToken ct);
        Task<IReadOnlyList<ArticleSummaryDto>> GetAdminArticles(string? q, string? status, CancellationToken ct);
    }
}
