using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public class CacheOptions
    {
        /// <summary>
        /// 是否使用二级缓存
        /// </summary>
        public bool UseL2Cache { get; set; } = false;
    }
}
