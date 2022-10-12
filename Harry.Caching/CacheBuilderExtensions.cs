using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Harry.Caching
{
    public static class CacheBuilderExtensions
    {
        /// <summary>
        /// 添加内存缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static ICacheBuilder AddMemoryCache(this ICacheBuilder builder, Action<MemoryCacheEntryOptions> setupAction = null)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            builder.Services.AddMemoryCache();
            builder.Services.TryAddSingleton<MemoryCacheProvider>();
            //设置默认参数
            builder.Services.Configure<MemoryCacheEntryOptions>(options =>
            {
                options.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            });
            ServiceDescriptor descriptor = ServiceDescriptor.Singleton<ICacheProvider, MemoryCacheProvider>(sp =>
            {
                var provider = sp.GetRequiredService<MemoryCacheProvider>();
                setupAction?.Invoke(provider.Options);
                return provider;
            });
            builder.Services.TryAddEnumerable(descriptor);
            return builder;
        }

        /// <summary>
        /// 添加分布式缓存
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static ICacheBuilder AddDistributedCache(this ICacheBuilder builder, Action<DistributedCacheEntryOptions> setupAction = null)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.TryAddSingleton<DistributedCacheProvider>();
            //设置默认参数
            builder.Services.Configure<DistributedCacheEntryOptions>(options =>
            {
                options.SetSlidingExpiration(TimeSpan.FromHours(2));
            });
            ServiceDescriptor descriptor = ServiceDescriptor.Singleton<ICacheProvider, DistributedCacheProvider>(sp =>
            {
                var provider = sp.GetRequiredService<DistributedCacheProvider>();
                setupAction?.Invoke(provider.Options);
                return provider;
            });
            builder.Services.TryAddEnumerable(descriptor);
            return builder;
        }

        /// <summary>
        /// 添加自定义缓存
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="builder"></param>
        /// <param name="func"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static ICacheBuilder AddCustomCache<TModel>(this ICacheBuilder builder, Func<IServiceProvider, string, Task<(bool, TModel)>> func, string categoryName = null)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            ArgumentNullException.ThrowIfNull(func, nameof(func));

            builder.Services.AddSingleton<ICacheProvider>(sp =>
            {
                var cache = new CustomCache<TModel>(key => func.Invoke(sp, key));
                var provider = new CustomCacheProvider<TModel>(cache, categoryName);
                return provider;
            });
            return builder;
        }
    }
}
