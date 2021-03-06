﻿using Harry.Caching;
using Harry.Caching.EventHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheOptions> options = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddLogging();
            services.AddOptions();
            services.AddMemoryCache();
            services.TryAddSingleton<ICache, Cache>();
            services.TryAddSingleton<IConverter, JsonConverter>();

            services.TryAddSingleton<CachingEventHandler>();

            if (options != null)
            {
                services.Configure(options);
            }
            return services;
        }
    }
}
