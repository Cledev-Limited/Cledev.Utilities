using Cledev.Core.Domain.Store.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        
        modelBuilder.ApplyConfiguration(new TestItemEntityTypeConfiguration());
        
        // modelBuilder
        //     .Entity<TestItem>()
        //     .ToTable(name: "Items");
        //
        // modelBuilder
        //     .Entity<TestSubItem>()
        //     .ToTable(name: "SubItems");
    }

    public DbSet<TestItem> Items { get; set; } = null!;
    public DbSet<TestSubItem> SubItems { get; set; } = null!;
}

internal class TestItemEntityTypeConfiguration : IEntityTypeConfiguration<TestItem>
{
    public void Configure(EntityTypeBuilder<TestItem> entityTypeBuilder)
    {
        entityTypeBuilder.ToTable("Items");
        // Other configuration

        var navigation = entityTypeBuilder.Metadata.FindNavigation(nameof(TestItem.SubItems));

        // EF access the SubItem collection property through its backing field
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
        // Other configuration
    }
}
