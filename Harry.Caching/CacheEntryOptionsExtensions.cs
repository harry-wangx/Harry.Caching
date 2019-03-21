using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public static class CacheEntryOptionsExtensions
    {
        public static MemoryCacheEntryOptions ToMemoryCacheEntryOptions(this CacheEntryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };
        }

        public static DistributedCacheEntryOptions ToDistributedCacheEntryOptions(this CacheEntryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return new DistributedCacheEntryOptions() {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };
        }
    }
}
