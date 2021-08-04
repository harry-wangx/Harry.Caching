namespace Harry.Caching
{
    public class CustomCache<TModel> : ICache<TModel>
    {
        private readonly Func<string, (bool, TModel)> _func;
        public CustomCache(Func<string, (bool, TModel)> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public Task<(bool, TModel)> GetAsync(string key, CancellationToken token = default)
        {
            var result = _func(key);
            return Task.FromResult(result);
        }

        public Task SetAsync(string key, TModel value, CancellationToken token = default)
        {
            /*无操作*/
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            /*无操作*/
            return Task.CompletedTask;
        }
    }
}
