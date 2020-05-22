using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mashup.Api.Models.JsonModels
{
    public class CoverImages
    {
        public string AlbumId { get; set; }

        [JsonProperty("images")]
        public List<CoverImage> Images { get; set; }
    }

    public class CoverImage
    {
        [JsonProperty("front")]
        public bool Front { get; set; }

        [JsonProperty("approved")]
        public bool Approved { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
