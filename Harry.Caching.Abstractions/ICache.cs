namespace Harry.Caching
{
    public interface ICache<TModel>
    {
        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<(bool, TModel)> GetAsync(string key, CancellationToken token = default);

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        Task SetAsync(string key, TModel value, CancellationToken token = default);

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="key"></param>
        Task RemoveAsync(string key, CancellationToken token = default);
    }
}
