using Cledev.Core.Domain.Store.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Core.Domain.Store.EF;

public class DomainDbContext : DbContext
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<AggregateEntity>()
            .ToTable(name: "Aggregates");
        
        builder
            .Entity<EventEntity>()
            .ToTable(name: "Events");
    }
    
    public DbSet<AggregateEntity> Aggregates { get; set; } = null!;
    public DbSet<EventEntity> Events { get; set; } = null!;
}
