using Newtonsoft.Json;

namespace Mashup.Api.Models.JsonModels
{
    public class ReleaseGroup
    {
        [JsonProperty("primary-type")]
        public string PrimaryType { get; set; }

        //[JsonProperty("primary-type-id")]
        //public string PrimaryTypeId { get; set; }        

        [JsonProperty("title")]
        public string Title { get; set; }        
        
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("first-release-date")]
        public string FirstReleaseDate { get; set; }

        public string CoverImage { get; set; }
    }
}
