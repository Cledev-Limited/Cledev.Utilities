﻿using Cledev.Core.Domain.Store.EF.Entities;
using Cledev.Core.Domain.Store.EF.Extensions;
using Cledev.Core.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cledev.Core.Domain.Store.EF;

public abstract class DomainDbContext : IdentityDbContext<IdentityUser>
{
    protected DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<AggregateEntity>()
            .ToTable(name: "Aggregates");
        
        modelBuilder
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
    public static async Task<Result<T>> GetAggregate<T>(this DomainDbContext domainDbContext, string id, ReadMode readMode = ReadMode.Weak, int fromVersionNumber = 1) where T : IAggregateRoot
    {
        if (readMode is ReadMode.Strong)
        {
            var eventEntities = await domainDbContext.Events.Where(x => x.AggregateRootId == id && x.Sequence >= fromVersionNumber).ToListAsync();
            if (eventEntities.Count == 0)
            {
                return new Failure(ErrorCodes.NotFound);
            }
            var aggregate = Activator.CreateInstance<T>();          
            aggregate.Apply(eventEntities.Select(x => (IDomainEvent)JsonConvert.DeserializeObject(x.Data, Type.GetType(x.Type)!)!));
            return aggregate;
        }
        
        var aggregateEntity = await domainDbContext.Aggregates.FirstOrDefaultAsync(x => x.Id == id);
        if (aggregateEntity is null)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        return JsonConvert.DeserializeObject<T>(aggregateEntity.Data)!;
    }
    
    public static async Task<Result> SaveAggregate(this DomainDbContext domainDbContext, IAggregateRoot aggregateRoot, int expectedVersionNumber, CancellationToken cancellationToken = default)
    {
        // TODO: Validate expected version number against current aggregate version
        
        var aggregateEntity = aggregateRoot.ToAggregateEntity(expectedVersionNumber + 1);
        if(expectedVersionNumber > 0)
        {
            domainDbContext.Update(aggregateRoot);
            domainDbContext.Aggregates.Update(aggregateEntity);
        }
        else
        {
            domainDbContext.Add(aggregateRoot);
            domainDbContext.Aggregates.Add(aggregateEntity);
        }

        var domainEvents = aggregateRoot.UncommittedEvents.ToArray();
        for (var i = 0; i < domainEvents.Length; i++)
        {
            var domainEvent = domainEvents[i];
            var eventEntity = domainEvent.ToEventEntity(expectedVersionNumber + i + 1);
            domainDbContext.Events.Add(eventEntity);
        }

        await domainDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}

public enum ReadMode
{
    Weak,
    Strong
}
