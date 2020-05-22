using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;
using Mashup.Api.Helpers;
using Mashup.Api.Interfaces;
using Mashup.Api.Models;
using Mashup.Api.Models.JsonModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Mashup.Api.Services
{
    public class MashupService : IMashupService
    {
        private readonly ILogger<MashupService> _logger;
        private readonly ICacheHelper _cacheHelper;
        private readonly IConfiguration _configuration;

        public MashupService(ILogger<MashupService> logger, ICacheHelper cacheHelper, IConfiguration configuration)
        {
            _logger = logger;
            _cacheHelper = cacheHelper;
            _configuration = configuration;
        }

        public async Task<MashupResultModel> BuildMashupModel(string mbId, string langCode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(mbId)) throw new ArgumentNullException(nameof(mbId), "MbId cannot be null or empty.");

            var result = new MashupResultModel();

            try
            {
                var musicBrainzData = await GetMusicBrainzData(mbId, cancellationToken);
                var albums = await GetAlbumsWithCover(musicBrainzData.ReleaseGroups, mbId, cancellationToken);
                var wikiData = await GetWikiData(musicBrainzData, cancellationToken);
                var wikipediaData = await GetWikipediaData(wikiData, mbId, langCode, cancellationToken);

                result.Name = musicBrainzData.Name;
                result.MbId = musicBrainzData.Id;
                result.Country = musicBrainzData.Country;
                result.MusicType = musicBrainzData.Disambiguation;
                result.LifeSpan = musicBrainzData.LifeSpan;
                result.Description = wikipediaData.Description;
                result.Albums = albums.ToList();


            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }


            return result;
        }

        public async Task<MusicBrainsJsonModel> GetMusicBrainzData(string mbId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(mbId))
                throw new ArgumentNullException(nameof(mbId));

            var cachedMusicBrainzData = _cacheHelper.GetFromCache<MusicBrainsJsonModel>($"MusicBrainz_{mbId}");
            if (cachedMusicBrainzData != null) return cachedMusicBrainzData;

            var baseUrl = _configuration.GetValue<string>("ApiUrls:MusicBrainz").Replace("{mbId}", mbId);
            try
            {
                var client = new RestClient(baseUrl)
                {
                    Timeout = -1
                };

                var response = await client.ExecuteGetAsync<MusicBrainsJsonModel>(new RestRequest(Method.GET), cancellationToken);

                if (response.Data != null)
                {
                    _cacheHelper.AddToCache($"MusicBrainz_{mbId}", response.Data, TimeSpan.FromDays(1));
                    return response.Data;
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return null;
        }

        public async Task<List<SiteLink>> GetWikiData(MusicBrainsJsonModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (model == null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null.");


            var id = MashupServiceHelper.ParseWikiDataId(model.Relations);
            if (string.IsNullOrEmpty(id)) return null;

            var cachedWikiData = _cacheHelper.GetFromCache<List<SiteLink>>($"WikiData_{id}");
            if (cachedWikiData != null) return cachedWikiData;


            var baseUrl = _configuration.GetValue<string>("ApiUrls:WikiData").Replace("{id}", id);

            try
            {
                var client = new RestClient(baseUrl)
                {
                    Timeout = -1
                };

                var response = await client.ExecuteGetAsync(new RestRequest(Method.GET), cancellationToken);

                var parsedResponse = JObject.Parse(response.Content);

                var siteLinkResults = parsedResponse["entities"][id]["sitelinks"].Children().ToList();

                var siteLinksModel = siteLinkResults.Select(siteLink => siteLink.Children().First().ToObject<SiteLink>()).ToList();


                if (siteLinksModel.Any())
                {
                    _cacheHelper.AddToCache($"WikiData_{id}", siteLinksModel, TimeSpan.FromDays(1));
                    return siteLinksModel;
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return null;
        }

        public async Task<WikipediaJsonModel> GetWikipediaData(IReadOnlyCollection<SiteLink> siteLinks, string mbId, string lanCode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var cachedWikipediaData = _cacheHelper.GetFromCache<WikipediaJsonModel>($"Wikipedia_{mbId}_{lanCode}");
            if (cachedWikipediaData != null) return cachedWikipediaData;

            if (siteLinks == null || !siteLinks.Any())
                throw new ArgumentNullException(nameof(siteLinks));

            var currentSiteLink = MashupServiceHelper.GetSiteLinkForCurrentLang(siteLinks, lanCode);

            if (currentSiteLink == null || string.IsNullOrEmpty(currentSiteLink.Title)) return null;

            var baseUrl = _configuration.GetValue<string>("ApiUrls:Wikipedia").Replace("{title}", HttpUtility.UrlEncode(currentSiteLink.Title));

            try
            {
                var client = new RestClient(baseUrl)
                {
                    Timeout = -1
                };

                var response = await client.ExecuteGetAsync(new RestRequest(Method.GET), cancellationToken);

                var parsedResponse = JObject.Parse(response.Content);

                var parsedResult = parsedResponse["query"]["pages"].Children().ToList();

                var model = parsedResult.Children().First().ToObject<WikipediaJsonModel>();

                if (model != null)
                {
                    _cacheHelper.AddToCache($"Wikipedia_{mbId}_{lanCode}", model, TimeSpan.FromDays(1));
                    return model;
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            return null;
        }

        public async Task<IEnumerable<ReleaseGroup>> GetAlbumsWithCover(IReadOnlyCollection<ReleaseGroup> releaseGroups, string mbId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (releaseGroups == null || !releaseGroups.Any())
                throw new ArgumentNullException(nameof(releaseGroups));


            var cachedAlbums = _cacheHelper.GetFromCache<IEnumerable<ReleaseGroup>>($"CoverArt_{mbId}");
            if (cachedAlbums != null) return cachedAlbums;


            var albums = releaseGroups.Where(x => x.PrimaryType.ToLowerInvariant().Equals("album")).ToList();

            try
            {
                var client = new RestClient(_configuration.GetValue<string>("ApiUrls:CoverArt"))
                {
                    Timeout = -1
                };

                var requests = albums.Select(album => RequestForCoverAsync(album, client, cancellationToken)).ToArray();
                var responseResults = await Task.WhenAll(requests);

                foreach (var responseResult in responseResults.Where(x => x.Data != null))
                {
                    var releaseGroup = albums.First(x => x.Id == responseResult.Data.AlbumId);
                    releaseGroup.CoverImage = responseResult.Data.Images.FirstOrDefault(x => x.Front)?.Image;
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }

            _cacheHelper.AddToCache($"CoverArt_{mbId}", albums, TimeSpan.FromDays(1));

            return albums;
        }


        private static async Task<IRestResponse<CoverImages>> RequestForCoverAsync(ReleaseGroup album, IRestClient client, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new RestRequest(album.Id, Method.GET);
            var response = await client.ExecuteAsync<CoverImages>(request, cancellationToken);

            if (response.Data != null)
            {
                response.Data.AlbumId = album.Id;
            }

            return response;
        }
    }
}
