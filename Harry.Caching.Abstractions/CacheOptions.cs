using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public class CacheOptions
    {
        public Action<DistributedCacheEntryOptions> DefaultDistributedOptions { get; } = new Action<DistributedCacheEntryOptions>(options => options.SlidingExpiration = TimeSpan.FromHours(2));

        public Action<MemoryCacheEntryOptions> DefaultMemoryOptions { get; } = new Action<MemoryCacheEntryOptions>(options => options.SlidingExpiration = TimeSpan.FromMinutes(5));
    }
}
