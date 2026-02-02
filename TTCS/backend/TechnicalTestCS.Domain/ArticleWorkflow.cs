namespace TechnicalTestCS.Domain
{
    public static class ArticleWorkflow
    {
        private static readonly Dictionary<ArticleStatus, ArticleStatus[]> Allowed = new()
        {
            [ArticleStatus.Draft] = [ArticleStatus.Pending],
            [ArticleStatus.Pending] = [ArticleStatus.Accepted, ArticleStatus.Rejected],
            [ArticleStatus.Rejected] = [ArticleStatus.Pending],
            [ArticleStatus.Accepted] = [],
        };

        public static bool CanTransition(ArticleStatus from, ArticleStatus to)
            => Allowed.TryGetValue(from, out var next) && next.Contains(to);
    }

}
