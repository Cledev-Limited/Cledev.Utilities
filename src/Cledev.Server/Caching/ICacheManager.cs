namespace Cledev.Server.Caching;

public interface ICacheManager
{
    Task<T?> GetOrSetAsync<T>(string cacheKey, TimeSpan cacheTime, Func<Task<T>> acquireAsync);

    void Remove(string key);
}
