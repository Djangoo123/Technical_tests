
using System.Text.Json.Serialization;

namespace TechnicalTestCS.Infrastructure.External.Dtos
{
    public class ImageFormatsExternalDto
    {
        [JsonPropertyName("large")]
        public ImageFormatExternalDto? Large { get; set; }

        [JsonPropertyName("medium")]
        public ImageFormatExternalDto? Medium { get; set; }

        [JsonPropertyName("small")]
        public ImageFormatExternalDto? Small { get; set; }

        [JsonPropertyName("thumbnail")]
        public ImageFormatExternalDto? Thumbnail { get; set; }
    }
}
