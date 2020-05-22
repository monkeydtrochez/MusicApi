using Newtonsoft.Json;

namespace Mashup.Api.Models.JsonModels
{
    public class LifeSpan
    {

        [JsonProperty("begin")]
        public string Begin { get; set; }     
        
        [JsonProperty("ended")]
        public bool Ended { get; set; } 
        
        [JsonProperty("end")]
        public string End { get; set; }
    }
}
