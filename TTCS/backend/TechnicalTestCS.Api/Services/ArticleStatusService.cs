using Microsoft.EntityFrameworkCore;
using TechnicalTestCS.Api.Interfaces;
using TechnicalTestCS.Domain;
using TechnicalTestCS.Infrastructure.Persistence;

namespace TechnicalTestCS.Api.Services
{
    public sealed class ArticleStatusService : IArticleStatusService
    {
        private readonly AppDbContext _db;

        public ArticleStatusService(AppDbContext db) => _db = db;

        public async Task<Dictionary<int, ArticleStatus>> GetStatusesByIds(IReadOnlyCollection<int> ids, CancellationToken ct)
        {
            if (ids.Count == 0) return [];

            return await _db.ArticleStatuses
                .Where(x => ids.Contains(x.ArticleId))
                .ToDictionaryAsync(x => x.ArticleId, x => x.Status, cancellationToken: ct);
        }

        public async Task<ArticleStatus> GetStatus(int id, CancellationToken ct)
        {
            ArticleStatusEntity? row = await _db.ArticleStatuses.FindAsync([id], ct);
            return row?.Status ?? ArticleStatus.Draft;
        }

        public async Task<ArticleStatus> UpdateStatus(int id, ArticleStatus next, CancellationToken ct)
        {
            ArticleStatusEntity? row = await _db.ArticleStatuses.FindAsync([id], ct);
            ArticleStatus current = row?.Status ?? ArticleStatus.Draft;

            if (!ArticleWorkflow.CanTransition(current, next))
                throw new InvalidTransitionException(current, next);

            if (row is null)
            {
                row = new ArticleStatusEntity
                {
                    ArticleId = id,
                    Status = next,
                    UpdatedAtUtc = DateTime.UtcNow
                };
                _db.ArticleStatuses.Add(row);
            }
            else
            {
                row.Status = next;
                row.UpdatedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);
            return next;
        }
    }
}
