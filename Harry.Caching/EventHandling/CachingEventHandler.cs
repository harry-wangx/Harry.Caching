using Harry.Caching.Events;
using Harry.EventBus.Handlers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Harry.Caching.EventHandling
{
    /// <summary>
    /// 此处理程序用于移除对应的本地缓存
    /// </summary>
    public class CachingEventHandler : IEventHandler<CachingEvent>
    {
        private readonly IMemoryCache _memoryCache;
        public CachingEventHandler(IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
        }

        public Task Handle(CachingEvent @event)
        {
            if (@event.CacheKey == null)
            {
                return Task.CompletedTask;
            }

            var data = this._memoryCache.Get<ICacheData>(@event.CacheKey);
            if (data != null && data.Version == @event.Version)
            {
                //数据一致,不需要更新
                return Task.CompletedTask;
            }

            //尝试移除对象(即使没有,也不会抛异常)
            this._memoryCache.Remove(@event.CacheKey);

            return Task.CompletedTask;
        }
    }
}
