using System.Net.Http.Json;

namespace Cledev.Client.Services;

public class ApiServiceAnonymous(HttpClient httpClient)
{
    public async Task<T?> GetFromJsonAsync<T>(string requestUri)
    {
        return await httpClient.GetFromJsonAsync<T>(requestUri);
    }
}