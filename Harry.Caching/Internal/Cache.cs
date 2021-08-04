namespace Harry.Caching.Internal
{
    internal sealed class Cache<TModel> : ICache<TModel>
    {
        private readonly LinkedList<ICache<TModel>> _providers;

        public Cache(IList<ICache<TModel>> providers)
        {
            ArgumentNullException.ThrowIfNull(providers, nameof(providers));
            _providers = new LinkedList<ICache<TModel>>(providers);
        }

        #region ICacheProvider接口实现

        /// <summary>
        /// 尝试获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public async Task<(bool, TModel)> GetAsync(string key, CancellationToken token = default)
        {
            //从上向下查找
            var next = _providers.First;
            while (next != null)
            {
                (var result, var model) = await next.Value.GetAsync(key, token);
                if (result)
                {
                    //找到数据,更新之前的缓存
                    var previous = next.Previous;
                    while (previous != null)
                    {
                        await previous.Value.SetAsync(key, model, token);
                        previous = previous.Previous;
                    }
                    return (result, model);
                }
                else
                {
                    //未找到数据
                    next = next.Next;
                }
            }
            return (false, default);
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public async Task SetAsync(string key, TModel value, CancellationToken token = default)
        {
            //从底层向上更新
            var provider = _providers.Last;
            while (provider != null)
            {
                await provider.Value.SetAsync(key, value, token);
                provider = provider.Previous;
            }
        }

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key"></param>
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            //从底层向上删除
            var provider = _providers.Last;
            while (provider != null)
            {
                await provider.Value.RemoveAsync(key, token);
                provider = provider.Previous;
            }
        }
        #endregion
    }
}
