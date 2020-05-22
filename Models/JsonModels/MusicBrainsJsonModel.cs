using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mashup.Api.Models.JsonModels
{
    public class MusicBrainsJsonModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("disambiguation")]
        public string Disambiguation { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("life-span")]
        public LifeSpan LifeSpan { get; set; }

        [JsonProperty("relations")]
        public List<Relations> Relations { get; set; }

        [JsonProperty("release-groups")]
        public List<ReleaseGroup> ReleaseGroups { get; set; }
    }
}
