using System.Net;
using System.Net.Http.Json;
using TechnicalTestCS.Infrastructure.External.Dtos;
using TechnicalTestCS.Infrastructure.External.Interfaces;

namespace TechnicalTestCS.Infrastructure.External
{
    public sealed class ExternalArticlesClient : IExternalArticlesClient
    {
        private readonly HttpClient _http;

        public ExternalArticlesClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IReadOnlyList<ArticleExternalDto>> GetArticles(CancellationToken ct)
        {
            HttpResponseMessage? res = await _http.GetAsync("/articles", ct);
            if (!res.IsSuccessStatusCode)
                throw new ExternalApiException((int)res.StatusCode, "Failed to fetch articles list from external API.");

            List<ArticleExternalDto>? items = await res.Content.ReadFromJsonAsync<List<ArticleExternalDto>>(cancellationToken: ct);
            return items ?? [];
        }

        public async Task<ArticleExternalDto?> GetArticle(int id, CancellationToken ct)
        {
            HttpResponseMessage? res = await _http.GetAsync($"/articles/{id}", ct);

            if (res.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!res.IsSuccessStatusCode)
                throw new ExternalApiException((int)res.StatusCode, $"Failed to fetch article {id} from external API.");

            return await res.Content.ReadFromJsonAsync<ArticleExternalDto>(cancellationToken: ct);
        }
    }

}
