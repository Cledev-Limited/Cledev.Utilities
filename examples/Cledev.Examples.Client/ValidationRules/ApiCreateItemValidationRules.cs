using Cledev.Client.Services;
using Cledev.Examples.Shared;

namespace Cledev.Examples.Client.ValidationRules;

public class ApiCreateItemValidationRules(ApiService apiService) : ICreateItemValidationRules
{
    public async Task<bool> IsItemNameUnique(string name)
    {
        return await apiService.GetFromJsonAsync<bool>($"api/items/is-name-unique/{name}");
    }
}