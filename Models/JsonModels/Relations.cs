using System.Text.Json.Serialization;

namespace Mashup.Api.Models.JsonModels
{
    public class Relations
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public RelationsUrl Url { get; set; }

    }

    public class RelationsUrl
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("resource")]
        public string Resource { get; set; }
    }
}
