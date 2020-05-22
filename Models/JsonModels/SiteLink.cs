using System.Text.Json.Serialization;

namespace Mashup.Api.Models.JsonModels
{
    public class SiteLink
    {
        [JsonPropertyName("site")]
        public string Site { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
