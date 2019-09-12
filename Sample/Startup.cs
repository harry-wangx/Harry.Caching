using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Harry.Caching;
using Harry.Caching.EventHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Harry.EventBus;
using Harry.Caching.Events;

namespace Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCache();
            services.AddDistributedMemoryCache();

            //如要使用缓存自动更新,必须开启事件总线,并注册CachingEventHandler订阅
            services.AddEventBus(builder => builder.UseMemory().InitEventBus(bus => bus.Subscribe<CachingEvent, CachingEventHandler>()));
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/set", builder =>
            {
                builder.Run(async (context) =>
                {
                    var now = DateTime.Now.ToString();
                    context.RequestServices.GetRequiredService<ICache>().Set("now", now, options => options.SlidingExpiration = TimeSpan.FromMinutes(2));
                    await context.Response.WriteAsync(now);
                });
            });

            app.Map("/get", builder =>
            {
                builder.Run(async (context) =>
                {
                    var now = context.RequestServices.GetRequiredService<ICache>().Get<string>("now") ?? "null";
                    await context.Response.WriteAsync(now);
                });
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
