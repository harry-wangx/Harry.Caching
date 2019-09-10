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
        private readonly Lazy<IDistributedCache> _distributedCache;
        private readonly Lazy<IConverter> _converter;
        private readonly ILogger _logger;
        public Cache(IServiceProvider serviceProvider, IMemoryCache memoryCache, IOptions<CacheOptions> optionsAccessor, ILoggerFactory loggerFactory)
        {
            this._serviceProvider = serviceProvider;
            this._memoryCache = memoryCache;
            this._options = optionsAccessor.Value;
            this._logger = loggerFactory.CreateLogger<Cache>();

            _distributedCache = new Lazy<IDistributedCache>(() => this._serviceProvider.GetRequiredService<IDistributedCache>());
            _converter = new Lazy<IConverter>(() => this._serviceProvider.GetRequiredService<IConverter>());
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

        public void Set<T>(string key, T value, CacheEntryOptions options)
        {
            ValidateCacheKey(key);

            var cacheData = getAndSetCacheData(key, value, options);

            if (useL2Cache())
            {
                _distributedCache.Value.Set(key, _converter.Value.Serialize(cacheData), options.ToDistributedCacheEntryOptions());
                publish(key, cacheData.Version);
            }
        }

        public async Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default)
        {
            ValidateCacheKey(key);

            var cacheData = getAndSetCacheData(key, value, options);

            if (useL2Cache())
            {
                await _distributedCache.Value.SetAsync(key, _converter.Value.Serialize(cacheData), options.ToDistributedCacheEntryOptions(), token);
                publish(key, cacheData.Version);
            }
        }

        public void Remove(string key)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (useL2Cache())
            {
                _distributedCache.Value.Remove(key);
                publish(key, null);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (useL2Cache())
            {
                await _distributedCache.Value.RemoveAsync(key);
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
        private CacheData<T> getAndSetCacheData<T>(string key, T value, CacheEntryOptions options)
        {
            //封装缓存数据
            var cacheData = new CacheData<T>(value);
            //获取内存缓存配置项
            var memoryOptions = options.ToMemoryCacheEntryOptions();
            this._memoryCache.Set(key, cacheData, memoryOptions);
            return cacheData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void publish(string key, string version)
        {
            this._serviceProvider.GetService<IEventBusFactory>()?.CreateEventBus()?.Publish(new HarryCachingEvent(key, version));
        }


        private CacheData<T> getCache<T>(string key)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (useL2Cache())
            {
                try
                {
                    var bytes = _distributedCache.Value.Get(key);
                    if (bytes != null && bytes.Length > 0)
                    {
                        var data = _converter.Value.Deserialize<CacheData<T>>(bytes);
                        this._memoryCache.Set(key, data, _options.DefaultEntryOptions.ToMemoryCacheEntryOptions());
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"获取二级缓存失败.key:{key} 错误:{ex.Message}");
                }
            }
            return default;
        }

        private async Task<CacheData<T>> getCacheAsync<T>(string key, CancellationToken token = default)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (useL2Cache())
            {
                try
                {
                    var bytes = await _distributedCache.Value.GetAsync(key, token);
                    if (bytes != null && bytes.Length > 0)
                    {
                        var data = _converter.Value.Deserialize<CacheData<T>>(bytes);
                        this._memoryCache.Set(key, data, _options.DefaultEntryOptions.ToMemoryCacheEntryOptions());
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"异步获取二级缓存失败.key:{key} 错误:{ex.Message}");
                }
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool useL2Cache()
        {
            return this._options.UseL2Cache;
        }


    }
}
