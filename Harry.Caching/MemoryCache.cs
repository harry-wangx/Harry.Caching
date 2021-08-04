using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace Harry.Caching
{
    public class MemoryCache<TModel> : ICache<TModel>
    {
        private readonly string _prefix;

        public MemoryCache(MemoryCacheProvider provider, string categoryName)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            CategoryName = !string.IsNullOrEmpty(categoryName) ? categoryName :
                throw new ArgumentNullException(nameof(categoryName));
            _prefix = categoryName + ":";
        }

        public MemoryCacheProvider Provider { get; private set; }

        public string CategoryName { get; private set; }

        #region ICache 接口实现

        /// <summary>
        /// 尝试获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<(bool, TModel)> GetAsync(string key, CancellationToken token = default)
        {
            var cacheKey = GetCacheKey(key);
            if (Provider.Cache.TryGetValue(cacheKey, out TModel entry))
            {
                //正常拿到缓存数据(缓冲数据有可能为null)
                return Task.FromResult((true, entry));
            }
            else
            {
                return Task.FromResult((false, entry));
            }
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Task SetAsync(string key, TModel value, CancellationToken token = default)
        {
            var cacheKey = GetCacheKey(key);
            Provider.Cache.Set(cacheKey, value, Provider.Options);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key"></param>
        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            var cacheKey = GetCacheKey(key);
            Provider.Cache.Remove(cacheKey);
            return Task.CompletedTask;
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetCacheKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _prefix + key;
        }
    }
}
