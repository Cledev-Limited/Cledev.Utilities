using Cledev.Core.Data;
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
            .ToTable(name: "Events");
            
        modelBuilder
            .Entity<AggregateEventEntity>()
            .ToTable(name: "AggregateEvents")
            .HasKey("AggregateEntityId", "EventEntityId");
    }
    
    public DbSet<AggregateEntity> Aggregates { get; set; } = null!;
    public DbSet<EventEntity> Events { get; set; } = null!;
    public DbSet<AggregateEventEntity> AggregateEvents { get; set; } = null!;
    
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

// TODO: Move to a domain repository
public static class DomainDbContextExtensions
{
    public static async Task<Result<T>> GetAggregate<T>(this DomainDbContext domainDbContext, string id, ReadMode readMode = ReadMode.Weak, int upToVersionNumber = -1) where T : IAggregateRoot =>
        readMode is ReadMode.Strong || upToVersionNumber > 0
            ? await domainDbContext.GetAggregateStrongView<T>(id, upToVersionNumber)
            : await domainDbContext.GetAggregateWeakView<T>(id);

    private static async Task<Result<T>> GetAggregateStrongView<T>(this DomainDbContext domainDbContext, string id, int upToVersionNumber = -1) where T : IAggregateRoot
    {
        var eventEntities = upToVersionNumber > 0
            ? await domainDbContext.AggregateEvents.AsNoTracking()
                .Where(aggregateEvent => aggregateEvent.AggregateEntityId == id && aggregateEvent.Sequence <= upToVersionNumber)
                .OrderBy(aggregateEvent => aggregateEvent.Sequence)
                .Select(aggregateEvent => aggregateEvent.EventEntity)
                .ToListAsync()
            : await domainDbContext.AggregateEvents.AsNoTracking()
                .Where(aggregateEvent => aggregateEvent.AggregateEntityId == id)
                .OrderBy(aggregateEvent => aggregateEvent.Sequence)
                .Select(aggregateEvent => aggregateEvent.EventEntity)
                .ToListAsync();
        
        if (eventEntities.Count == 0)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        
        var aggregate = Activator.CreateInstance<T>();          
        aggregate.LoadFromHistory(eventEntities.Select(eventEntity => (IDomainEvent)JsonConvert.DeserializeObject(eventEntity.Data, Type.GetType(eventEntity.Type)!)!));
        return aggregate;
    }

    private static async Task<Result<T>> GetAggregateWeakView<T>(this DomainDbContext domainDbContext, string id) where T : IAggregateRoot
    {
        var aggregateEntity = await domainDbContext.Aggregates.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id);
        if (aggregateEntity is null)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        return (T)JsonConvert.DeserializeObject(aggregateEntity.Data, Type.GetType(aggregateEntity.Type)!)!;
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
        var currentVersionNumber = await domainDbContext.AggregateEvents.AsNoTracking().CountAsync(aggregateEventEntity => aggregateEventEntity.AggregateEntityId == aggregateRoot.Id, cancellationToken);
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
            domainDbContext.Events.Add(domainEvent.ToEventEntity());
            domainDbContext.AggregateEvents.Add(new AggregateEventEntity
            {
                AggregateEntityId = aggregateRoot.Id,
                EventEntityId = domainEvent.Id,
                Sequence = startingVersionNumber + i
            });
        }
    }
    
    private static void TrackEntities(this DbContext domainDbContext, AggregateRoot aggregateRoot)
    {
        foreach (var entity in aggregateRoot.UncommittedEntities)
        {
            switch (entity.State)
            {
                case State.Added:
                    domainDbContext.Add(entity);
                    break;
                case State.Modified:
                    domainDbContext.Update(entity);
                    break;
                case State.Deleted:
                    domainDbContext.Remove(entity);
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
