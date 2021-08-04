using Microsoft.Extensions.DependencyInjection;

namespace Harry.Caching.Internal
{
    internal sealed class CacheBuilder : ICacheBuilder
    {
        public CacheBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

    }
}
