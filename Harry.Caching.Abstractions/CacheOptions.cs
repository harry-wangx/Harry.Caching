using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public class CacheOptions
    {
        public DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions { get; } = new DistributedCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2) };

        public MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; } = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(5) };
    }
}
