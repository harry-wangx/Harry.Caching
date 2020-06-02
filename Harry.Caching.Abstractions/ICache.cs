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
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">无缓存时,执行此函数获取对象</param>
        /// <param name="memoryOptionsAction">内存缓存配置</param>
        /// <param name="distributedOptionsAction">分布式缓存配置</param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> factory = null,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null);

        /// <summary>
        /// 异步获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">无缓存时,执行此函数获取对象</param>
        /// <param name="memoryOptionsAction">内存缓存配置</param>
        /// <param name="distributedOptionsAction">分布式缓存配置</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key, Func<T> factory = null,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null,
            CancellationToken? token = null);

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存的对象</param>
        /// <param name="memoryOptionsAction">内存缓存配置</param>
        /// <param name="distributedOptionsAction">分布式缓存配置</param>
        void Set<T>(string key, T value,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null);

        /// <summary>
        /// 异步设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存的对象</param>
        /// <param name="memoryOptionsAction">内存缓存配置</param>
        /// <param name="distributedOptionsAction">分布式缓存配置</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null,
            CancellationToken? token = null);

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key">缓存键</param>
        void Remove(string key);

        /// <summary>
        /// 异步移除缓存对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task RemoveAsync(string key, CancellationToken? token = null);
    }
}
