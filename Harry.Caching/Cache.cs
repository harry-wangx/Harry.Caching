using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Harry.Caching.Events;
using Harry.EventBus;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Harry.Caching
{
    public class Cache : ICache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;
        private readonly CacheOptions _options;
        private readonly IDistributedCache _distributedCache;
        private readonly IConverter _converter;
        private readonly ILogger _logger;
        public Cache(IServiceProvider serviceProvider, IMemoryCache memoryCache, IOptions<CacheOptions> optionsAccessor, ILoggerFactory loggerFactory)
        {
            this._serviceProvider = serviceProvider;
            this._memoryCache = memoryCache;
            this._options = optionsAccessor.Value;
            this._logger = loggerFactory.CreateLogger<Cache>();

            if (_options.UseL2Cache)
            {
                _distributedCache = this._serviceProvider.GetRequiredService<IDistributedCache>();
                _converter = this._serviceProvider.GetRequiredService<IConverter>();
            }
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="factory">无缓存时,执行此函数获取对象</param>
        /// <param name="memoryOptionsAction">内存缓存配置</param>
        /// <param name="distributedOptionsAction">分布式缓存配置</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> factory = null,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null)
        {
            ValidateCacheKey(key);

            var cacheData = GetCacheData<T>(key, memoryOptionsAction);

            if (cacheData != null)
            {
                //此时有可能返回null
                return cacheData.Data;
            }
            else if (factory != null)
            {
                var data = factory.Invoke();
                //即使data为null,依然缓存
                this.Set(key, data, memoryOptionsAction, distributedOptionsAction);
                return data;
            }
            else
                return default;
        }

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
        public async Task<T> GetAsync<T>(string key, Func<T> factory = null,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null,
            CancellationToken? token = null)
        {
            ValidateCacheKey(key);

            var cacheData = await GetCacheDataAsync<T>(key, memoryOptionsAction, token ?? CancellationToken.None);

            if (cacheData != null)
            {
                //此时有可能返回null
                return cacheData.Data;
            }
            else if (factory != null)
            {
                var data = factory.Invoke();
                //即使data为null,依然缓存
                await this.SetAsync(key, data, memoryOptionsAction, distributedOptionsAction, token);
                return data;
            }
            else
                return default;
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存的对象</param>
        /// <param name="memoryOptionsAction">内存缓存配置</param>
        /// <param name="distributedOptionsAction">分布式缓存配置</param>
        public void Set<T>(string key, T value,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null)
        {
            ValidateCacheKey(key);

            var cacheData = setMemoryCache(key, value, memoryOptionsAction);

            if (_options.UseL2Cache)
            {
                var distributedOptions = GetDefaultDistributedOptions();
                distributedOptionsAction?.Invoke(distributedOptions);
                _distributedCache.Set(key, _converter.Serialize(cacheData), distributedOptions);
                publish(key, cacheData.Version);
            }
        }

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
        public async Task SetAsync<T>(string key, T value,
            Action<MemoryCacheEntryOptions> memoryOptionsAction = null,
            Action<DistributedCacheEntryOptions> distributedOptionsAction = null,
            CancellationToken? token = null)
        {
            ValidateCacheKey(key);

            var cacheData = setMemoryCache(key, value, memoryOptionsAction);

            if (_options.UseL2Cache)
            {
                var distributedOptions = GetDefaultDistributedOptions();
                distributedOptionsAction?.Invoke(distributedOptions);
                await _distributedCache.SetAsync(key, _converter.Serialize(cacheData), distributedOptions, token ?? CancellationToken.None);
                publish(key, cacheData.Version);
            }
        }

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (_options.UseL2Cache)
            {
                _distributedCache.Remove(key);
                publish(key, null);
            }
        }

        /// <summary>
        /// 异步移除缓存对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string key, CancellationToken? token = null)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (_options.UseL2Cache)
            {
                await _distributedCache.RemoveAsync(key, token ?? CancellationToken.None);
                publish(key, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateCacheKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        /// <summary>
        /// 获取默认内存缓存配置
        /// </summary>
        /// <returns></returns>
        private MemoryCacheEntryOptions GetDefaultMemoryOptions()
        {
            var options = new MemoryCacheEntryOptions() { };
            if (_options.DefaultMemoryOptions != null)
            {
                _options.DefaultMemoryOptions.Invoke(options);
            }
            else
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(5);
            }

            return options;
        }

        /// <summary>
        /// 获取默认分布式缓存配置
        /// </summary>
        /// <returns></returns>
        private DistributedCacheEntryOptions GetDefaultDistributedOptions()
        {
            var options = new DistributedCacheEntryOptions() { };
            if (_options.DefaultDistributedOptions != null)
            {
                _options.DefaultDistributedOptions.Invoke(options);
            }
            else
            {
                options.SlidingExpiration = TimeSpan.FromHours(2);
            }

            return options;
        }


        private CacheData<T> setMemoryCache<T>(string key, T value, Action<MemoryCacheEntryOptions> memoryOptionsAction)
        {
            //options不允许为空
            MemoryCacheEntryOptions memoryOptions = GetDefaultMemoryOptions();
            memoryOptionsAction?.Invoke(memoryOptions);
            //封装缓存数据
            var cacheData = new CacheData<T>(value);
            //获取内存缓存配置项
            this._memoryCache.Set(key, cacheData, memoryOptions);
            return cacheData;
        }

        /// <summary>
        /// 获取CacheData
        /// </summary>
        private CacheData<T> GetCacheData<T>(string key, Action<MemoryCacheEntryOptions> memoryOptionsAction)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (_options.UseL2Cache)
            {
                var bytes = _distributedCache.Get(key);
                if (bytes != null && bytes.Length > 0)
                {
                    var data = _converter.Deserialize<CacheData<T>>(bytes);
                    setMemoryCache(key, data, memoryOptionsAction);
                    return data;
                }
            }
            return default;
        }

        /// <summary>
        /// 异步获CacheData
        /// </summary>
        private async Task<CacheData<T>> GetCacheDataAsync<T>(string key, Action<MemoryCacheEntryOptions> memoryOptionsAction, CancellationToken token)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (_options.UseL2Cache)
            {
                var bytes = await _distributedCache.GetAsync(key, token);
                if (bytes != null && bytes.Length > 0)
                {
                    var data = _converter.Deserialize<CacheData<T>>(bytes);
                    setMemoryCache(key, data, memoryOptionsAction);
                    return data;
                }
            }
            return default;
        }

        /// <summary>
        /// 发布缓存事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="version"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void publish(string key, string version)
        {
            this._serviceProvider.GetService<IEventBusFactory>()?.CreateEventBus()?.Publish(new CachingEvent(key, version), typeof(CachingEvent).GetFullName());
        }
    }
}
