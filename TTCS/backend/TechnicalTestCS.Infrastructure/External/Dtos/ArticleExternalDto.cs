
using System.Text.Json.Serialization;

namespace TechnicalTestCS.Infrastructure.External.Dtos
{
    public class ArticleExternalDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("image")]
        public ImageExternalDto? Image { get; set; }

        [JsonPropertyName("banner")]
        public ImageExternalDto? Banner { get; set; }

        [JsonPropertyName("partners")]
        public List<PartnerExternalDto>? Partners { get; set; }
    }
}
