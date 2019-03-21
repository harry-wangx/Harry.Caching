using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
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
        public Cache(IServiceProvider serviceProvider, IMemoryCache memoryCache, IOptions<CacheOptions> optionsAccessor)
        {
            this._serviceProvider = serviceProvider;
            this._memoryCache = memoryCache;
            this._options = optionsAccessor.Value;

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

            var cacheData = await getCacheAsync<T>(key);
            if (cacheData != null)
                return cacheData.Data;

            return default;
        }

        public void Set<T>(string key, T value, CacheEntryOptions options)
        {
            ValidateCacheKey(key);

            //封装缓存数据
            var cacheData = new CacheData<T>(value);
            //获取内存缓存配置项
            //todo:未来可通过 memoryOptions的ExpirationTokens,实现缓存对像的过期,如收到二级缓存或数据源更新消息时
            var memoryOptions = options.ToMemoryCacheEntryOptions();

            this._memoryCache.Set(key, cacheData, memoryOptions);

            if (useL2Cache())
            {
                _distributedCache.Value.Set(key, _converter.Value.Serialize(cacheData), options.ToDistributedCacheEntryOptions());
            }
        }

        public async Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken token = default)
        {
            ValidateCacheKey(key);
            //封装缓存数据
            var cacheData = new CacheData<T>(value);
            //获取内存缓存配置项
            //todo:未来可通过 memoryOptions的ExpirationTokens,实现缓存对像的过期,如收到二级缓存或数据源更新消息时
            var memoryOptions = options.ToMemoryCacheEntryOptions();

            this._memoryCache.Set(key, cacheData, memoryOptions);

            if (useL2Cache())
            {
                await _distributedCache.Value.SetAsync(key, _converter.Value.Serialize(cacheData), options.ToDistributedCacheEntryOptions());
            }
        }

        public void Remove(string key)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (useL2Cache())
            {
                _distributedCache.Value.Remove(key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            ValidateCacheKey(key);

            this._memoryCache.Remove(key);

            if (useL2Cache())
            {
                await _distributedCache.Value.RemoveAsync(key);
            }
        }

        private static void ValidateCacheKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        private CacheData<T> getCache<T>(string key)
        {
            if (this._memoryCache.TryGetValue(key, out CacheData<T> value))
            {
                return value;
            }
            if (useL2Cache())
            {
                var bytes = _distributedCache.Value.Get(key);
                if (bytes != null && bytes.Length > 0)
                {
                    return _converter.Value.Deserialize<CacheData<T>>(bytes);
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
                var bytes = await _distributedCache.Value.GetAsync(key, token);
                if (bytes != null && bytes.Length > 0)
                {
                    return _converter.Value.Deserialize<CacheData<T>>(bytes);
                }
            }
            return default;
        }

        private bool useL2Cache()
        {
            return this._options.UseL2Cache;
        }
    }
}
