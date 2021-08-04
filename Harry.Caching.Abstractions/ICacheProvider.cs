namespace Harry.Caching
{
    public interface ICacheProvider : IDisposable
    {
        public int Order { get; set; }

        ICache<TModel> CreateCache<TModel>(string categoryName);
    }
}
