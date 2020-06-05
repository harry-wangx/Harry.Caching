using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public class CacheOptions
    {
        /// <summary>
        /// 分布式缓存配置
        /// </summary>
        public Action<DistributedCacheEntryOptions> DefaultDistributedOptions { get; } = new Action<DistributedCacheEntryOptions>(options => options.SlidingExpiration = TimeSpan.FromHours(2));

        /// <summary>
        /// 内存缓存配置
        /// </summary>
        public Action<MemoryCacheEntryOptions> DefaultMemoryOptions { get; } = new Action<MemoryCacheEntryOptions>(options => options.SlidingExpiration = TimeSpan.FromMinutes(5));

        /// <summary>
        /// 是否使用2级缓存
        /// </summary>
        public bool UseL2Cache { get; set; } = false;
    }
}
