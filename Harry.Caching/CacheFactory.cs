using Harry.Caching.Internal;

namespace Harry.Caching
{
    public class CacheFactory : ICacheFactory
    {
        private readonly Dictionary<string, object> _dicCache = new(StringComparer.Ordinal);
        private readonly List<ICacheProvider> _providers;
        private readonly object _sync = new object();
        private volatile bool _disposed;

        public CacheFactory(IEnumerable<ICacheProvider> sources)
        {
            ArgumentNullException.ThrowIfNull(sources);

            _providers = sources.OrderBy(m => m.Order).ToList();
        }

        public ICache<TModel> CreateCache<TModel>(string categoryName)
        {
            if (CheckDisposed())
            {
                throw new ObjectDisposedException(nameof(CacheFactory));
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("分类名称不能为空", nameof(categoryName));
            }

            lock (_sync)
            {
                if (_dicCache.TryGetValue(categoryName, out object? obj))
                {
                    if (obj is ICache<TModel> cache)
                    {
                        return cache;
                    }
                    else
                    {
                        throw new Exception($"获取名称为:{categoryName}的{typeof(TModel)}类型的Cache失败,返回类型为:{(obj == null ? "null" : obj.GetType())}");
                    }
                }
                else
                {
                    List<ICache<TModel>> list = new(_providers.Count);
                    foreach (var provider in _providers)
                    {
                        var cacheItem = provider.CreateCache<TModel>(categoryName);
                        if (cacheItem != null)
                        {
                            list.Add(cacheItem);
                        }
                    }
                    var cache = new Cache<TModel>(list);
                    _dicCache[categoryName] = cache;
                    return cache;
                }
            }

        }

        protected virtual bool CheckDisposed() => _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                foreach (var provider in _providers)
                {
                    try
                    {
                        provider.Dispose();
                    }
                    catch
                    {
                        // Swallow exceptions on dispose
                    }
                }
            }
        }
    }
}
