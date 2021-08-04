using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Harry.Caching
{
    public class DistributedCacheProvider : ICacheProvider
    {
        private volatile bool _disposed;
        public DistributedCacheProvider(IDistributedCache cache,
            DistributedCacheEntryOptions options,
            IConverter converter)
        {
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public DistributedCacheProvider(IDistributedCache cache,
            IOptions<DistributedCacheEntryOptions> optionsAccessor,
            IConverter converter)
            : this(cache, optionsAccessor.Value, converter)
        {

        }

        public IDistributedCache Cache { get; private set; }

        public DistributedCacheEntryOptions Options { get; private set; }

        public int Order { get; set; } = 100;

        public IConverter Converter { get; private set; }

        public ICache<TModel> CreateCache<TModel>(string categoryName)
        {
            return new DistributedCache<TModel>(this, categoryName);
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
