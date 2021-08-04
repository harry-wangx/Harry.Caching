using System.Runtime.CompilerServices;

namespace Harry.Caching
{
    public class DistributedCache<TModel> : ICache<TModel>
    {
        private readonly string _prefix;
        public DistributedCache(DistributedCacheProvider provider, string categoryName)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            CategoryName = !string.IsNullOrEmpty(categoryName) ? categoryName :
                throw new ArgumentNullException(nameof(categoryName));
            _prefix = categoryName + ":";
        }

        public DistributedCacheProvider Provider { get; private set; }

        public string CategoryName { get; private set; }


        #region ICache 接口实现

        /// <summary>
        /// 尝试获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<(bool, TModel)> GetAsync(string key, CancellationToken token = default)
        {
            var cacheKey = GetCacheKey(key);
            var data = await Provider.Cache.GetAsync(cacheKey, token);
            if (data == null || data.Length <= 0)
            {
                return (false, default);
            }

            var model = Provider.Converter.Deserialize<TModel>(data);
            return (true, model);
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Task SetAsync(string key, TModel value, CancellationToken token = default)
        {
            //如果value为null,直接退出.如果频繁访问不存在的资源,会穿透此层缓存.
            if (value == null) return Task.CompletedTask;

            var cacheKey = GetCacheKey(key);
            var data = Provider.Converter.Serialize(value);
            return Provider.Cache.SetAsync(cacheKey, data, Provider.Options, token);
        }

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key"></param>
        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            var cacheKey = GetCacheKey(key);
            return Provider.Cache.RemoveAsync(cacheKey, token);
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
