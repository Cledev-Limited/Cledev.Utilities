﻿using Cledev.Core.Data;
using Cledev.Core.Domain.Store.EF.Entities;
using Cledev.Core.Domain.Store.EF.Extensions;
using Cledev.Core.Extensions;
using Cledev.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cledev.Core.Domain.Store.EF;

public abstract class DomainDbContext(
    DbContextOptions<DomainDbContext> options,
    TimeProvider timeProvider,
    IHttpContextAccessor httpContextAccessor)
    : IdentityDbContext<IdentityUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<AggregateEntity>()
            .ToTable(name: "Aggregates");
        
        modelBuilder
            .Entity<EventEntity>()
            .ToTable(name: "Events")
            .HasOne(x => x.AggregateEntity)
            .WithMany(x => x.EventEntities)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
    
    public DbSet<AggregateEntity> Aggregates { get; set; } = null!;
    public DbSet<EventEntity> Events { get; set; } = null!;
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = timeProvider.GetUtcNow();
        var userId = httpContextAccessor.GetCurrentUserId();

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
                        Entry(auditableEntity).Property(entity => entity.CreatedDate).IsModified = false;
                        Entry(auditableEntity).Property(entity => entity.CreatedBy).IsModified = false;
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
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ContractResolver = new PrivateSetterContractResolver()
    };
    
    public static async Task<Result<T>> GetAggregate<T>(this DomainDbContext domainDbContext, string id, ReadMode readMode = ReadMode.Weak, int upToVersionNumber = -1, CancellationToken cancellationToken = default) where T : IAggregateRoot =>
        readMode is ReadMode.Strong || upToVersionNumber > 0
            ? await domainDbContext.GetAggregateStrongView<T>(id, upToVersionNumber, cancellationToken: cancellationToken)
            : await domainDbContext.GetAggregateWeakView<T>(id, cancellationToken: cancellationToken);

    private static async Task<Result<T>> GetAggregateStrongView<T>(this DomainDbContext domainDbContext, string id, int upToVersionNumber = -1, CancellationToken cancellationToken = default) where T : IAggregateRoot
    {
        var eventEntities = upToVersionNumber > 0
            ? await domainDbContext.Events.AsNoTracking()
                .Where(eventEntity => eventEntity.AggregateEntityId == id && eventEntity.Sequence <= upToVersionNumber)
                .OrderBy(eventEntity => eventEntity.Sequence)
                .ToListAsync(cancellationToken: cancellationToken)
            : await domainDbContext.Events.AsNoTracking()
                .Where(eventEntity => eventEntity.AggregateEntityId == id)
                .OrderBy(eventEntity => eventEntity.Sequence)
                .ToListAsync(cancellationToken: cancellationToken);
        
        if (eventEntities.Count == 0)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        
        var aggregate = Activator.CreateInstance<T>();          
        aggregate.LoadFromHistory(eventEntities.Select(eventEntity => (DomainEvent)JsonConvert.DeserializeObject(eventEntity.Data, Type.GetType(eventEntity.Type)!, JsonSerializerSettings)!));
        return aggregate;
    }

    private static async Task<Result<T>> GetAggregateWeakView<T>(this DomainDbContext domainDbContext, string id, CancellationToken cancellationToken = default) where T : IAggregateRoot
    {
        var aggregateEntity = await domainDbContext.Aggregates.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken: cancellationToken);
        if (aggregateEntity is null)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        return (T)JsonConvert.DeserializeObject(aggregateEntity.Data, Type.GetType(aggregateEntity.Type)!, JsonSerializerSettings)!;
    }
    
    public static async Task<Result> SaveAggregate(this DomainDbContext domainDbContext, AggregateRoot aggregateRoot, int expectedVersionNumber, CancellationToken cancellationToken = default)
    {
        var startingVersionNumberResult = await domainDbContext.GetStartingVersionNumber(aggregateRoot, expectedVersionNumber, cancellationToken);
        if (startingVersionNumberResult.IsNotSuccess)
        {
            return startingVersionNumberResult.Failure!;
        }
        var startingVersionNumber = startingVersionNumberResult.Value;

        domainDbContext.TrackAggregate(aggregateRoot, startingVersionNumber);
        domainDbContext.TrackEvents(aggregateRoot, startingVersionNumber);
        domainDbContext.TrackEntities(aggregateRoot);

        await domainDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    private static async Task<Result<int>> GetStartingVersionNumber(this DomainDbContext domainDbContext, IAggregateRoot aggregateRoot, int expectedVersionNumber, CancellationToken cancellationToken = default)
    {
        var currentVersionNumber = await domainDbContext.Events.AsNoTracking().CountAsync(eventEntity => eventEntity.AggregateEntityId == aggregateRoot.Id, cancellationToken);
        if (currentVersionNumber != expectedVersionNumber)
        {
            return new Failure(Title: "Concurrency exception");
        }
        return expectedVersionNumber + 1;
    }

    private static void TrackAggregate(this DomainDbContext domainDbContext, IAggregateRoot aggregateRoot, int versionNumber)
    {
        var aggregateEntity = aggregateRoot.ToAggregateEntity(version: versionNumber);
        if(versionNumber > 1)
        {
            domainDbContext.Aggregates.Update(aggregateEntity);
        }
        else
        {
            domainDbContext.Aggregates.Add(aggregateEntity);
        }
    }
    
    private static void TrackEvents(this DomainDbContext domainDbContext, IAggregateRoot aggregateRoot, int startingVersionNumber)
    {
        var domainEvents = aggregateRoot.UncommittedEvents.ToArray();
        for (var i = 0; i < domainEvents.Length; i++)
        {
            var domainEvent = domainEvents[i];
            var eventEntity = domainEvent.ToEventEntity(version: startingVersionNumber + i);
            domainDbContext.Events.Add(eventEntity);
        }
    }
    
    private static void TrackEntities(this DbContext domainDbContext, AggregateRoot aggregateRoot)
    {
        foreach (var dbEntity in aggregateRoot.UncommittedEntities)
        {
            switch (dbEntity.State)
            {
                case State.Added:
                    domainDbContext.Add(dbEntity.Data);
                    break;
                case State.Modified:
                    domainDbContext.Update(dbEntity.Data);
                    break;
                case State.Deleted:
                    domainDbContext.Remove(dbEntity.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

public enum ReadMode
{
    Weak,
    Strong
}
