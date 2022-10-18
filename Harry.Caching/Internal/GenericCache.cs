namespace Harry.Caching.Internal
{
    internal sealed class GenericCache<TModel> : ICache<TModel>
    {
        private readonly ICache<TModel> _cache;
        public GenericCache(ICacheFactory cacheFactory)
        {
            ArgumentNullException.ThrowIfNull(cacheFactory, nameof(cacheFactory));
            _cache = cacheFactory.CreateCache<TModel>();
        }

        public Task<(bool, TModel)> GetAsync(string key, CancellationToken token = default)
        {
            return _cache.GetAsync(key, token);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return _cache.RemoveAsync(key, token);
        }

        public Task SetAsync(string key, TModel value, CancellationToken token = default)
        {
            return _cache.SetAsync(key, value, token);
        }
    }
}
