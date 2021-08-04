namespace Harry.Caching
{
    public interface ICacheFactory : IDisposable
    {
        /// <summary>
        /// 创建一个<see cref="ICache<TModel>">实例
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="categoryName">分类名称</param>
        /// <returns></returns>
        ICache<TModel> CreateCache<TModel>(string categoryName);
    }
}
