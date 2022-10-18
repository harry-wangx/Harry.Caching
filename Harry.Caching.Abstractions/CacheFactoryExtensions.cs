namespace Harry.Caching
{
    public static class CacheFactoryExtensions
    {
        public static ICache<TModel> CreateCache<TModel>(this ICacheFactory factory)
        {
            ArgumentNullException.ThrowIfNull(factory, nameof(factory));
            return factory.CreateCache<TModel>(TypeNameHelper.GetTypeDisplayName(typeof(TModel)));
        }
        public static ICache<TModel> CreateCacheWithSuffix<TModel>(this ICacheFactory factory, string suffix)
        {
            ArgumentNullException.ThrowIfNull(factory, nameof(factory));
            if (string.IsNullOrWhiteSpace(suffix))
                throw new ArgumentNullException(nameof(suffix));

            return factory.CreateCache<TModel>(TypeNameHelper.GetTypeDisplayName(typeof(TModel)) + ":" + suffix.Trim());
        }
    }
}
