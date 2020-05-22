using System.Collections.Generic;
using System.Linq;
using Mashup.Api.Models.JsonModels;

namespace Mashup.Api.Helpers
{
    public static class MashupServiceHelper
    {
        public static string ParseWikiDataId(List<Relations> relations)
        {
            var wikiRelationsUrl = relations.FirstOrDefault(x => x.Type.ToLower() == "wikidata")?.Url;
            var id = wikiRelationsUrl?.Resource.Split('/').Last();

            return id;
        }

        public static SiteLink GetSiteLinkForCurrentLang(IReadOnlyCollection<SiteLink> siteLinks, string langCode)
        {
            if (string.IsNullOrEmpty(langCode))
                langCode = "enwiki";

            var siteLink = siteLinks.FirstOrDefault(x => x.Site.ToLowerInvariant().Equals(langCode.ToLowerInvariant()));

            return siteLink;
        }
    }
}
