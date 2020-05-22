using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mashup.Api.Models
{
    public class RequestModel
    {
        /// <summary>
        /// MusicBrainz id (mbid)
        /// </summary>
        /// /// <example>7dc8f5bd-9d0b-4087-9f73-dc164950bbd8</example>
        [Required]
        [DefaultValue("7dc8f5bd-9d0b-4087-9f73-dc164950bbd8")]
        public string MbId { get; set; }

        /// <summary>
        /// Wikidata language code
        /// </summary>
        /// <example>enwiki</example>
        [DefaultValue("enwiki")]
        public string LangCode { get; set; }
    }
}
