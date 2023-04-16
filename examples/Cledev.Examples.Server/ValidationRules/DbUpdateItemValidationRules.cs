using Cledev.Example.Server.Data;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.ValidationRules;

public class DbUpdateItemValidationRules : IUpdateItemValidationRules
{
    private readonly ApplicationDbContext _dbContext;

    public DbUpdateItemValidationRules(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<bool> IsItemNameUnique(Guid id, string name) =>
        await _dbContext.Items.AnyAsync(item => item.Id != id && item.Name == name) is false;
}