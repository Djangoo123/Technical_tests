using TechnicalTestCS.Domain;

namespace TechnicalTestCS.Infrastructure.Persistence
{
    public class ArticleStatusEntity
    {
        public int ArticleId { get; set; } // id CMS
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
