using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.ValidationRules;

public class DbUpdateItemValidationRules(ApplicationDbContext dbContext) : IUpdateItemValidationRules
{
    public async Task<bool> IsItemNameUnique(Guid id, string name) =>
        await dbContext.Items.AnyAsync(item => item.Id != id && item.Name == name) is false;
}