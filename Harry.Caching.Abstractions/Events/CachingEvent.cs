namespace Harry.Caching.Events
{
    /// <summary>
    /// 此事件用于设置分布式缓存后,通知其它服务器从分布式缓存更新值
    /// </summary>
    public class CachingEvent : Harry.EventBus.Event
    {
        protected CachingEvent() { }

        public CachingEvent(string cacheKey, string version)
        {
            this.CacheKey = cacheKey;
            this.Version = version;
        }
        /// <summary>
        /// 缓存KEY值
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// 版本号(如为null或空,则认为是删除操作)
        /// </summary>
        public string Version { get; set; }
    }
}
