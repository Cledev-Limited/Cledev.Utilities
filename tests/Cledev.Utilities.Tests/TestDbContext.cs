using Cledev.Core.Domain.Store.EF;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Utilities.Tests;

public sealed class TestDbContext : DomainDbContext
{
    public TestDbContext(DbContextOptions<DomainDbContext> options) : base(options)
    {
        // Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .Entity<TestItem>()
            .ToTable(name: "Items");
    }

    public DbSet<TestItem> Items { get; set; } = null!;
}
