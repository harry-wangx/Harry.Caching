using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Harry.Caching
{
    public static class CacheExtensions
    {
        public static TItem GetOrCreate<TItem>(this ICache cache, string key, Func<CacheEntryOptions, TItem> factory)
        {
            var result = cache.Get<TItem>(key);
            if (result != null)
            {
                return result;
            }

            CacheEntryOptions options = new CacheEntryOptions();
            result = factory.Invoke(options);
            cache.Set(key, result, options);

            return result;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this ICache cache, string key, Func<CacheEntryOptions, Task<TItem>> factory)
        {
            var result = await cache.GetAsync<TItem>(key);
            if (result != null)
            {
                return result;
            }

            CacheEntryOptions options = new CacheEntryOptions();
            result = await factory.Invoke(options);
            await cache.SetAsync(key, result, options);

            return result;
        }
    }
}
