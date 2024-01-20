using Cledev.Core.Domain.Store.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Core.Tests;

public sealed class TestDbContext : DomainDbContext
{
    public TestDbContext(DbContextOptions<DomainDbContext> options, TimeProvider timeProvider, IHttpContextAccessor httpContextAccessor) : base(options, timeProvider, httpContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<TestItemEntity>()
            .ToTable(name: "Items")
            .HasMany(x => x.TestSubItems)
            .WithOne(x => x.TestItem);
        
        modelBuilder
            .Entity<TestSubItemEntity>()
            .ToTable(name: "SubItems");
    }

    public DbSet<TestItemEntity> Items { get; set; } = null!;
    public DbSet<TestSubItemEntity> SubItems { get; set; } = null!;
}
