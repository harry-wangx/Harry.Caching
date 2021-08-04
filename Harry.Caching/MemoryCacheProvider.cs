using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Harry.Caching
{
    public class MemoryCacheProvider : ICacheProvider
    {
        public MemoryCacheProvider(IMemoryCache cache, IOptions<MemoryCacheEntryOptions> optionsAccessor)
            : this(cache, optionsAccessor.Value)
        {

        }

        public MemoryCacheProvider(IMemoryCache cache, MemoryCacheEntryOptions options)
        {
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IMemoryCache Cache { get; private set; }

        public MemoryCacheEntryOptions Options { get; private set; }

        public int Order { get; set; } = 0;

        public ICache<TModel> CreateCache<TModel>(string categoryName)
        {
            return new MemoryCache<TModel>(this, categoryName);
        }

        public void Dispose()
        {
            //这一层不做任何处理
            return;
        }
    }
}
