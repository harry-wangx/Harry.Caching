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
        private readonly bool useL2Cache;
        public Cache(IServiceProvider serviceProvider, IMemoryCache memoryCache, IOptions<CacheOptions> optionsAccessor, ILoggerFactory loggerFactory)
        {
            this._serviceProvider = serviceProvider;
            this._memoryCache = memoryCache;
            this._options = optionsAccessor.Value;
            this._logger = loggerFactory.CreateLogger<Cache>();

            _distributedCache = this._serviceProvider.GetService<IDistributedCache>();
            _converter = this._serviceProvider.GetService<IConverter>();

            useL2Cache = _distributedCache != null && _converter != null;
        }
        public T Get<T>(string key)
        {
            ValidateCacheKey(key);

            var value = getCache<T>(key);
            if (value != null)
                return value.Data;

            return default;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken token = default)
        {
            ValidateCacheKey(key);

            var cacheData = await getCacheAsync<T>(key, token);
            if (cacheData != null)
                return cacheData.Data;

            return default;
        }

        public void Set<T>(string key, T value, MemoryCacheEntryOptions memoryOptions, DistributedCacheEntryOptions distributedOptions = null)
        {
            ValidateCacheKey(key);

            var cacheData = setMemoryCach(key, value, memoryOptions);

            if (useL2Cache)
            {
                distributedOptions = distributedOptions ?? _options.DefaultDistributedCacheEntryOptions;
                _distributedCache.Set(key, _converter.Serialize(cacheData), distributedOptions);
                publish(key, cacheData.Version);
            }
        }

        public async Task SetAsync<T>(string key, T value, MemoryCacheEntryOptions memoryOptions, DistributedCacheEntryOptions distributedOptions = null, CancellationToken token = default)
        {
            ValidateCacheKey(key);

            var cacheData = setMemoryCach(key, value, memoryOptions);

            if (useL2Cache)
            {
                distributedOptions = distributedOptions ?? _options.DefaultDistributedCacheEntryOptions;
                await _distributedCache.SetAsync(key, _converter.Serialize(cacheData), distributedOptions, token);
                publish(key, cacheData.Version);
            }
        }

        public void Remove(string key)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (useL2Cache)
            {
                _distributedCache.Remove(key);
                publish(key, null);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (useL2Cache)
            {
                await _distributedCache.RemoveAsync(key);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private CacheData<T> setMemoryCach<T>(string key, T value, MemoryCacheEntryOptions options)
        {
            //options不允许为空
            options = options ?? _options.DefaultMemoryCacheEntryOptions;
            //封装缓存数据
            var cacheData = new CacheData<T>(value);
            //获取内存缓存配置项
            this._memoryCache.Set(key, cacheData, options);
            return cacheData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void publish(string key, string version)
        {
            this._serviceProvider.GetService<IEventBusFactory>()?.CreateEventBus()?.Publish(new CachingEvent(key, version), typeof(CachingEvent).GetFullName());
        }


        private CacheData<T> getCache<T>(string key)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (useL2Cache)
            {
                //try
                //{
                var bytes = _distributedCache.Get(key);
                if (bytes != null && bytes.Length > 0)
                {
                    var data = _converter.Deserialize<CacheData<T>>(bytes);
                    this._memoryCache.Set(key, data, _options.DefaultMemoryCacheEntryOptions);
                    return data;
                }
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, $"获取二级缓存失败.key:{key} 错误:{ex.Message}");
                //}
            }
            return default;
        }

        private async Task<CacheData<T>> getCacheAsync<T>(string key, CancellationToken token = default)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (useL2Cache)
            {
                //try
                //{
                var bytes = await _distributedCache.GetAsync(key, token);
                if (bytes != null && bytes.Length > 0)
                {
                    var data = _converter.Deserialize<CacheData<T>>(bytes);
                    this._memoryCache.Set(key, data, _options.DefaultMemoryCacheEntryOptions);
                    return data;
                }
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, $"异步获取二级缓存失败.key:{key} 错误:{ex.Message}");
                //}
            }
            return default;
        }
    }
}
