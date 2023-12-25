using Microsoft.Extensions.Caching.Memory;

namespace Cledev.Server.Caching;

public class MemoryCacheManager : ICacheManager
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheManager(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public async Task<T?> GetOrSetAsync<T>(string cacheKey, TimeSpan cacheTime, Func<Task<T>> acquireAsync)
    {
        if (_memoryCache.TryGetValue(cacheKey, out T? data))
        {
            return data;
        }

        data = await acquireAsync();

        var memoryCacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(cacheTime);

        _memoryCache.Set(cacheKey, data, memoryCacheEntryOptions);

        return data;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}
