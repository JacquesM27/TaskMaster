using Microsoft.Extensions.Caching.Memory;
using TaskMaster.Abstractions.Cache;

namespace TaskMaster.Infrastructure.Cache;

internal sealed class CacheStorage(IMemoryCache cache) : ICacheStorage
{
    public void Set<T>(string key, T value, TimeSpan? duration = null)
        => cache.Set(key, value, duration ?? TimeSpan.FromSeconds(5));

    public T? Get<T>(string key) => cache.Get<T>(key);
}