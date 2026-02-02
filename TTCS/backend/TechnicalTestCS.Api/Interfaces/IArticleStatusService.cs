using TechnicalTestCS.Domain;

namespace TechnicalTestCS.Api.Interfaces
{
    public interface IArticleStatusService
    {
        Task<Dictionary<int, ArticleStatus>> GetStatusesByIds(IReadOnlyCollection<int> ids, CancellationToken ct);
        Task<ArticleStatus> GetStatus(int id, CancellationToken ct);
        Task<ArticleStatus> UpdateStatus(int id, ArticleStatus next, CancellationToken ct);
    }
}
