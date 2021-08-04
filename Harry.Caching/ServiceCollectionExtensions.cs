using Harry.Caching;
using Harry.Caching.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<ICacheBuilder> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAddSingleton<ICacheFactory, CacheFactory>();
            services.TryAddSingleton<IConverter, JsonConverter>();

            var builder = new CacheBuilder(services);
            configure?.Invoke(builder);
            return services;
        }
    }
}
