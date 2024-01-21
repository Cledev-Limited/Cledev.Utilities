using Cledev.Client.Services;
using Cledev.Examples.Shared;

namespace Cledev.Examples.Client.ValidationRules;

public class ApiUpdateItemValidationRules(ApiService apiService) : IUpdateItemValidationRules
{
    public async Task<bool> IsItemNameUnique(Guid id, string name)
    {
        return await apiService.GetFromJsonAsync<bool>($"api/items/is-name-unique/{name}?id={id}");
    }
}