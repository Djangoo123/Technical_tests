using TechnicalTestCS.Domain;

namespace TechnicalTestCS.Api.Services
{
    public static class StatusParsing
    {
        public static string ToApiString(this ArticleStatus status)
            => status.ToString().ToLowerInvariant();

        public static bool TryParse(string input, out ArticleStatus status)
            => Enum.TryParse<ArticleStatus>(input, ignoreCase: true, out status);
    }
}
