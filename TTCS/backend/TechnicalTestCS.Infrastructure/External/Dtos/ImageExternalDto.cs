
using System.Text.Json.Serialization;

namespace TechnicalTestCS.Infrastructure.External.Dtos
{
    public class ImageExternalDto
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("alternativeText")]
        public string? AlternativeText { get; set; }

        [JsonPropertyName("formats")]
        public ImageFormatsExternalDto? Formats { get; set; }
    }
}
