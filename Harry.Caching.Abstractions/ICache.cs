using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harry.Caching
{
    public interface ICache
    {
        T Get<T>(string key);

        Task<T> GetAsync<T>(string key, CancellationToken token = default);

        void Set<T>(string key, T value, MemoryCacheEntryOptions memoryOptions, DistributedCacheEntryOptions distributedOptions = null);

        Task SetAsync<T>(string key, T value, MemoryCacheEntryOptions memoryOptions, DistributedCacheEntryOptions distributedOptions = null, CancellationToken token = default);

        void Remove(string key);

        Task RemoveAsync(string key, CancellationToken token = default);
    }
}
