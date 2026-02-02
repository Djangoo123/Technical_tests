using TechnicalTestCS.Api.Dtos;
using TechnicalTestCS.Api.Interfaces;
using TechnicalTestCS.Domain;
using TechnicalTestCS.Infrastructure.External.Dtos;
using TechnicalTestCS.Infrastructure.External.Interfaces;

namespace TechnicalTestCS.Api.Services
{
    public sealed class ArticlesService : IArticlesService
    {
        private readonly IExternalArticlesClient _external;
        private readonly IArticleStatusService _statusService;
        private readonly string _externalBaseUrl;

        public ArticlesService(
            IExternalArticlesClient external,
            IArticleStatusService statusService,
            IConfiguration config)
        {
            _external = external;
            _statusService = statusService;
            _externalBaseUrl = config["ExternalApi:BaseUrl"] ?? "https://cms-beta.happytal.com";
        }

        public async Task<IReadOnlyList<ArticleSummaryDto>> GetArticles(string? q, string? status, CancellationToken ct)
        {
            IReadOnlyList<ArticleExternalDto>? items = await _external.GetArticles(ct);

            // filter txt
            if (!string.IsNullOrWhiteSpace(q))
            {
                var filterText = q.Trim();
                items = [.. items.Where(a =>
                        (a.Title?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (a.Slug?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false))];
            }

            List<int>? ids = items.Select(a => a.Id).ToList();
            Dictionary<int, ArticleStatus>? map = await _statusService.GetStatusesByIds(ids, ct);

            List<ArticleSummaryDto>? results = [.. items.Select(a =>
            {
                ArticleStatus st = map.TryGetValue(a.Id, out var s) ? s : ArticleStatus.Draft;
                return new ArticleSummaryDto(
                    a.Id,
                    a.Title ?? $"Article {a.Id}",
                    a.Slug,
                    PickBestImageUrl(a.Image),
                    st.ToApiString());
            })];

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!StatusParsing.TryParse(status, out var wanted))
                    throw new ArgumentException("Unknown status filter.", nameof(status));

                results = results.Where(x => string.Equals(x.Status, wanted.ToApiString(), StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return results;
        }

        public async Task<IReadOnlyList<ArticleSummaryDto>> GetAdminArticles(string? q, string? status, CancellationToken ct)
            => await GetArticles(q, status, ct);

        public async Task<ArticleDetailDto?> GetArticle(int id, CancellationToken ct)
        {
            ArticleExternalDto? article = await _external.GetArticle(id, ct);
            if (article is null) return null;

            ArticleStatus st = await _statusService.GetStatus(id, ct);

            List<PartnerDto>? partners = [.. (article.Partners ?? [])
                .Select(p => new PartnerDto(
                    p.Id,
                    p.Name ?? $"Partner {p.Id}",
                    p.Website,
                    p.Email,
                    p.Phone,
                    p.Description,
                    PickBestImageUrl(p.Logo)))];

            return new ArticleDetailDto(
                article.Id,
                article.Title ?? $"Article {article.Id}",
                article.Slug,
                article.Content,
                PickBestImageUrl(article.Image),
                PickBestImageUrl(article.Banner),
                partners,
                st.ToApiString());
        }

        private string? PickBestImageUrl(ImageExternalDto? img)
        {
            if (img is null) return null;

            string? url =
                img.Formats?.Medium?.Url ??
                img.Formats?.Small?.Url ??
                img.Formats?.Thumbnail?.Url ??
                img.Url;

            if (string.IsNullOrWhiteSpace(url)) return null;

            // if relative URL > add prefix
            if (Uri.TryCreate(url, UriKind.Absolute, out _))
                return url;

            if (!url.StartsWith("/")) url = "/" + url;
            return _externalBaseUrl.TrimEnd('/') + url;
        }
    }
}
