using System.Net.Http.Json;

namespace Cledev.Client.Services;

public class ApiServiceAuthenticated(HttpClient httpClient)
{
    public async Task<T?> GetFromJsonAsync<T>(string requestUri)
    {
        return await httpClient.GetFromJsonAsync<T>(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data)
    {
        return await httpClient.PostAsJsonAsync(requestUri, data);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return await httpClient.PostAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        return await httpClient.DeleteAsync(requestUri);
    }
}