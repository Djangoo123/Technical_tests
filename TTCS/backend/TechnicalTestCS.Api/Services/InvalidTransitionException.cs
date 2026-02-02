using TechnicalTestCS.Domain;

namespace TechnicalTestCS.Api.Services
{
    public sealed class InvalidTransitionException : Exception
    {
        public ArticleStatus From { get; }
        public ArticleStatus To { get; }

        public InvalidTransitionException(ArticleStatus from, ArticleStatus to)
            : base($"Transition not allowed: {from} -> {to}")
        {
            From = from;
            To = to;
        }
    }
}
