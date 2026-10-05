using System.Text.Json.Serialization;

namespace NewPlus.Models
{
    public class ExtensionProfile
    {
        [JsonPropertyName("extension")]
        public string Extension { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;
    }
}