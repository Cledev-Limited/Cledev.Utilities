using Cledev.Example.Server.Data;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.ValidationRules;

public class DbCreateItemValidationRules : ICreateItemValidationRules
{
    private readonly ApplicationDbContext _dbContext;

    public DbCreateItemValidationRules(ApplicationDbContext dbContext) => 
        _dbContext = dbContext;

    public async Task<bool> IsItemNameUnique(string name) =>
        await _dbContext.Items.AnyAsync(item => item.Name == name) is false;
}