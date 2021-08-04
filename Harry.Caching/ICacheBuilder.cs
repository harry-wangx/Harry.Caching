using Microsoft.Extensions.DependencyInjection;

namespace Harry.Caching
{
    public interface ICacheBuilder
    {
        IServiceCollection Services { get; }
    }
}
