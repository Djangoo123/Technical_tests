using System.Text.Json.Serialization;

namespace TechnicalTestCS.Infrastructure.External.Dtos
{
    public class ImageFormatExternalDto
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
