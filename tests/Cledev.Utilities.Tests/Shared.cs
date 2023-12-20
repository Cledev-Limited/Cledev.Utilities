using Cledev.Core.Domain.Store.EF;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Utilities.Tests;

public static class Shared
{
    public static DbContextOptions<DomainDbContext> CreateContextOptions()
    {
        var builder = new DbContextOptionsBuilder<DomainDbContext>();
        builder.UseInMemoryDatabase("Cledev");
        return builder.Options;
    }
}
