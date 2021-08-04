namespace Harry.Caching
{
    public class CustomCacheProvider<T> : ICacheProvider
    {
        private volatile bool _disposed;
        private readonly ICache<T> _cache;
        private readonly string _categoryName;

        public CustomCacheProvider(ICache<T> cache, string categoryName = null)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrEmpty(categoryName))
            {
                _categoryName = TypeNameHelper.GetTypeDisplayName(typeof(T));
            }
            else
            {
                _categoryName = categoryName;
            }
        }

        public int Order { get; set; } = 1000;

        public ICache<TModel> CreateCache<TModel>(string categoryName)
        {
            if (!string.Equals(categoryName, _categoryName))
            {
                return null;
            }

            if (typeof(T) != typeof(TModel))
            {
                return null;
            }

            return (ICache<TModel>)_cache;
        }

        protected virtual bool CheckDisposed() => _disposed;

        protected virtual void OnDispose() { }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                try
                {
                    OnDispose();
                }
                catch
                {
                    // Swallow exceptions on dispose
                }
            }
        }
    }
}
