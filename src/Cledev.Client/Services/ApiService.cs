using Microsoft.AspNetCore.Components.Authorization;

namespace Cledev.Client.Services;

public class ApiService(
    AuthenticationStateProvider authenticationStateProvider,
    ApiServiceAnonymous anonymousService,
    ApiServiceAuthenticated authenticatedService)
{
    public async Task<T?> GetFromJsonAsync<T>(string requestUri)
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();

        if (authenticationState.User.Identity is { IsAuthenticated: true })
        {
            return await authenticatedService.GetFromJsonAsync<T>(requestUri);
        }

        return await anonymousService.GetFromJsonAsync<T>(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data)
    {
        return await authenticatedService.PostAsJsonAsync(requestUri, data);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return await authenticatedService.PostAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        return await authenticatedService.DeleteAsync(requestUri);
    }
}