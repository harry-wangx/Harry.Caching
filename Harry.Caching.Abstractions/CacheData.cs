using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Caching
{
    public class CacheData<T>
    {
        protected CacheData() { }

        public CacheData(T data)
        {
            this.Data = data;
            Version = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 缓存的数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 缓存版本号,用来判断是否需要更新缓存
        /// </summary>
        public string Version { get; set; }
    }
}
