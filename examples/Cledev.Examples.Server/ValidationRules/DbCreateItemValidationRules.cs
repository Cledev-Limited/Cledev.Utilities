using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.ValidationRules;

public class DbCreateItemValidationRules(ApplicationDbContext dbContext) : ICreateItemValidationRules
{
    public async Task<bool> IsItemNameUnique(string name) =>
        await dbContext.Items.AnyAsync(item => item.Name == name) is false;
}