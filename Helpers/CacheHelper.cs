using System;
using Mashup.Api.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Mashup.Api.Helpers
{
    public class CacheHelper : ICacheHelper
    {
        private readonly IMemoryCache _cache;

        public CacheHelper(IMemoryCache cache)
        {
            _cache = cache;
        }
        public  T GetFromCache<T>(string cacheKey) where T : class
        {
          var cacheEntry =  _cache.Get<T>(cacheKey);

            return cacheEntry;
        }

        public void AddToCache<T>(string cacheKey, T cacheEntry, TimeSpan expirationTime) where T : class
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(expirationTime);

            _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
        }
    }
}
