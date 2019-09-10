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

        void Set<T>(string key, T value, CacheEntryOptions options);

        Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default);

        void Remove(string key);

        Task RemoveAsync(string key, CancellationToken token = default);

        ///// <summary>
        ///// 通知更新
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="version"></param>
        //void Publish(string key, string version);
    }
}
