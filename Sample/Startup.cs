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
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCache(options => options.UseL2Cache = true);
            services.AddDistributedMemoryCache();
            services.AddSingleton<HarryCachingEventHandler>();
            services.AddEventBus(builder => builder.UseMemory().InitEventBus(bus => bus.Subscribe<HarryCachingEvent, HarryCachingEventHandler>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{

            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.Map("/set", builder =>
            {
                builder.Run(async (context) =>
                {
                    var now = DateTime.Now.ToString();
                    context.RequestServices.GetRequiredService<ICache>().Set("now", now, new CacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(2) });
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
        }
    }
}
