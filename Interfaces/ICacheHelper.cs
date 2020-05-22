using System;

namespace Mashup.Api.Interfaces
{
    public interface ICacheHelper
    {
        T GetFromCache<T>(string cacheKey) where T : class;
        void AddToCache<T>(string cacheKey, T cacheEntry, TimeSpan expirationTime) where T : class;
    }
}
