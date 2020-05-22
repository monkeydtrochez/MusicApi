using Newtonsoft.Json;

namespace Mashup.Api.Models.JsonModels
{
    public class WikipediaJsonModel
    {
        [JsonProperty("extract")]
        public string Description { get; set; }
    }
}
