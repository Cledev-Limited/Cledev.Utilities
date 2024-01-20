using Cledev.Examples.Server.Data.Entities;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cledev.Examples.Server.Data;

public sealed class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Item>()
            .ToTable(name: "Items", tableBuilder => tableBuilder.IsTemporal());
    }

    [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)]
    public static string SoundsLike(string parameter)
    {
        throw new NotImplementedException();
    }

    public DbSet<Item> Items { get; set; } = null!;
}
