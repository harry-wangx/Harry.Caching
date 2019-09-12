using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harry.Caching
{
    public static class CacheExtensions
    {
        public static TItem GetOrCreate<TItem>(this ICache cache, string key, Func<MemoryCacheEntryOptions, TItem> factory)
        {
            var result = cache.Get<TItem>(key);
            if (result != null)
            {
                return result;
            }

            MemoryCacheEntryOptions memoryOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(5) };

            result = factory.Invoke(memoryOptions);
            cache.Set(key, result, memoryOptions);

            return result;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this ICache cache, string key, Func<MemoryCacheEntryOptions, Task<TItem>> factory)
        {
            var result = await cache.GetAsync<TItem>(key);
            if (result != null)
            {
                return result;
            }

            MemoryCacheEntryOptions memoryOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(5) };

            result = await factory.Invoke(memoryOptions);
            await cache.SetAsync(key, result, memoryOptions);

            return result;
        }

        public static TItem Set<TItem>(this ICache cache, string key, TItem value)
        {
            cache.Set<TItem>(key, value, null);
            return value;
        }

        public static TItem Set<TItem>(this ICache cache, string key, TItem value, DateTimeOffset absoluteExpiration)
        {
            cache.Set<TItem>(key, value, new MemoryCacheEntryOptions { AbsoluteExpiration = absoluteExpiration });
            return value;
        }

        public static TItem Set<TItem>(this ICache cache, string key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
        {
            cache.Set<TItem>(key, value, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow });
            return value;
        }

        public static TItem Set<TItem>(this ICache cache, string key, TItem value, Action<MemoryCacheEntryOptions> options)
        {
            MemoryCacheEntryOptions memoryOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(5) };

            options?.Invoke(memoryOptions);

            cache.Set<TItem>(key, value, memoryOptions);
            return value;
        }
    }
}
