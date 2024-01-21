using Microsoft.Extensions.Caching.Memory;

namespace Cledev.Server.Caching;

public class MemoryCacheManager(IMemoryCache memoryCache) : ICacheManager
{
    public async Task<T?> GetOrSetAsync<T>(string cacheKey, TimeSpan cacheTime, Func<Task<T>> acquireAsync)
    {
        if (memoryCache.TryGetValue(cacheKey, out T? data))
        {
            return data;
        }

        data = await acquireAsync();

        var memoryCacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(cacheTime);

        memoryCache.Set(cacheKey, data, memoryCacheEntryOptions);

        return data;
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }
}
