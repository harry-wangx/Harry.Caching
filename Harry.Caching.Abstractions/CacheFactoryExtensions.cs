namespace Harry.Caching
{
    public static class CacheFactoryExtensions
    {
        public static ICache<TModel> CreateCache<TModel>(this ICacheFactory factory)
        {
            ArgumentNullException.ThrowIfNull(factory, nameof(factory));
            return factory.CreateCache<TModel>(TypeNameHelper.GetTypeDisplayName(typeof(TModel)));
        }
    }
}
