using Cledev.Core.Domain.Store.EF.Entities;
using Cledev.Core.Domain.Store.EF.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Core.Domain.Store.EF;

public abstract class DomainDbContext : IdentityDbContext<IdentityUser>
{
    protected DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
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
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Use date provider
        var utcNow = DateTimeOffset.UtcNow;
        
        // TODO: Use user id from http context
        var userId = "system";

        foreach (var changedEntity in ChangeTracker.Entries())
        {
            if (changedEntity.Entity is IAuditableEntity auditableEntity)
            {
                switch (changedEntity.State)
                {
                    case EntityState.Added:
                        auditableEntity.CreatedDate = utcNow;
                        auditableEntity.CreatedBy = userId;
                        auditableEntity.LastUpdatedDate = utcNow;
                        auditableEntity.LastUpdatedBy = userId;
                        break;
                    case EntityState.Modified:
                        Entry(auditableEntity).Property(x => x.CreatedDate).IsModified = false;
                        Entry(auditableEntity).Property(x => x.CreatedBy).IsModified = false;
                        auditableEntity.LastUpdatedDate = utcNow;
                        auditableEntity.LastUpdatedBy = userId;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

public static class DomainDbContextExtensions
{
    public static void AddDomainEntities(this DomainDbContext domainDbContext, IAggregateRoot aggregateRoot, int version, bool aggregateIsNew = false)
    {
        var aggregateEntity = aggregateRoot.ToAggregateEntity(version);
        if(aggregateIsNew)
        {
            domainDbContext.Aggregates.Add(aggregateEntity);
        }
        else
        {
            domainDbContext.Aggregates.Update(aggregateEntity);
        }
        // TODO: Add events
        // domainDbContext.Events.AddRange(aggregateRoot.UncommittedEvents.Select(x => x.ToEventEntity(aggregateEntity.Id)));
    }
}
