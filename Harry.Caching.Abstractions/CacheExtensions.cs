using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harry.Caching
{
    public static class CacheExtensions
    {
        public static T Get<T>(this ICache cache, string key, Func<T> factory, TimeSpan slidingExpiration)
        {
            return cache.Get<T>(key, factory
                , new Action<MemoryCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration)
                , new Action<DistributedCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration));
        }

        public static Task<T> GetAsync<T>(this ICache cache, string key, Func<T> factory, TimeSpan slidingExpiration, CancellationToken? token = null)
        {
            return cache.GetAsync<T>(key, factory
                , new Action<MemoryCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration)
                , new Action<DistributedCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration)
                , token);
        }
        public static void Set<T>(this ICache cache, string key, T value, TimeSpan slidingExpiration)
        {
            cache.Set<T>(key, value
                , new Action<MemoryCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration)
                , new Action<DistributedCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration));
        }

        public static Task SetAsync<T>(this ICache cache, string key, T value, TimeSpan slidingExpiration, CancellationToken? token = null)
        {
            return cache.SetAsync<T>(key, value
                , new Action<MemoryCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration)
                , new Action<DistributedCacheEntryOptions>(_ => _.SlidingExpiration = slidingExpiration)
                , token);
        }
    }
}
