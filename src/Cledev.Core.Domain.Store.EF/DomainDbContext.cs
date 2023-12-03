using Cledev.Core.Domain.Store.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Core.Domain.Store.EF;

public class DomainDbContext : DbContext
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder
            .Entity<EventEntity>()
            .ToTable(name: "DomainEvent");
    }
    
    public DbSet<EventEntity> Events { get; set; } = null!;
}
