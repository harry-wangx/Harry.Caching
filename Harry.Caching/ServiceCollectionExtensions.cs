using Harry.Caching;
using Harry.Caching.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCache(this IServiceCollection services, Action<ICacheBuilder> configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.TryAddSingleton<ICacheFactory, CacheFactory>();
            services.TryAddSingleton<IConverter, JsonConverter>();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ICache<>), typeof(GenericCache<>)));

            var builder = new CacheBuilder(services);
            configure?.Invoke(builder);
            return services;
        }
    }
}
