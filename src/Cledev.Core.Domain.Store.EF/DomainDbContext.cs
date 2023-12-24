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

public abstract class DomainDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected DomainDbContext(DbContextOptions<DomainDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
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
        var userId = _httpContextAccessor.CurrentUserId();

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
    public static async Task<Result<T>> GetAggregate<T>(this DomainDbContext domainDbContext, string id, ReadMode readMode = ReadMode.Weak) where T : IAggregateRoot =>
        readMode is ReadMode.Strong
            ? await domainDbContext.GetAggregateStrongView<T>(id)
            : await domainDbContext.GetAggregateWeakView<T>(id);

    private static async Task<Result<T>> GetAggregateStrongView<T>(this DomainDbContext domainDbContext, string id) where T : IAggregateRoot
    {
        var eventEntities = await domainDbContext.Events.AsNoTracking().Where(x => x.AggregateRootId == id).ToListAsync();
        if (eventEntities.Count == 0)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        var aggregate = Activator.CreateInstance<T>();          
        aggregate.LoadFromHistory(eventEntities.Select(x => (IDomainEvent)JsonConvert.DeserializeObject(x.Data, Type.GetType(x.Type)!)!));
        return aggregate;
    }

    private static async Task<Result<T>> GetAggregateWeakView<T>(this DomainDbContext domainDbContext, string id) where T : IAggregateRoot
    {
        var aggregateEntity = await domainDbContext.Aggregates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (aggregateEntity is null)
        {
            return new Failure(ErrorCodes.NotFound);
        }
        return (T)JsonConvert.DeserializeObject(aggregateEntity.Data, Type.GetType(aggregateEntity.Type)!)!;
    }
    
    public static async Task<Result> SaveAggregate(this DomainDbContext domainDbContext, AggregateRoot aggregateRoot, int expectedVersionNumber, CancellationToken cancellationToken = default)
    {
        // TODO: Validate expected version number against current aggregate version

        var aggregateEntity = aggregateRoot.ToAggregateEntity(version: expectedVersionNumber + 1);
        if(expectedVersionNumber > 0)
        {
            domainDbContext.Aggregates.Update(aggregateEntity);
        }
        else
        {
            domainDbContext.Aggregates.Add(aggregateEntity);
        }

        var domainEvents = aggregateRoot.UncommittedEvents.ToArray();
        for (var i = 0; i < domainEvents.Length; i++)
        {
            var domainEvent = domainEvents[i];
            var eventEntity = domainEvent.ToEventEntity(expectedVersionNumber + i + 1);
            domainDbContext.Events.Add(eventEntity);
        }

        foreach (var entity in aggregateRoot.ReadModels)
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
                    return new Failure(Title: "Invalid entity state");
            }
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
