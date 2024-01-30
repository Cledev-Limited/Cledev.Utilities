﻿using Cledev.Core.Data;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace Cledev.Core.Domain;

public interface IAggregateRoot
{
    string Id { get; set; }
    int Version { get; set; }
    IEnumerable<IDomainEvent> UncommittedEvents { get; }
    void LoadFromHistory(IEnumerable<IDomainEvent> events);
}

public abstract class AggregateRoot : IAggregateRoot
{
    public virtual string Id { get; set; } = null!;
    public virtual int Version { get; set; }
    
    [JsonIgnore]
    public IEnumerable<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();
    private readonly List<IDomainEvent> _uncommittedEvents = [];
    
    [JsonIgnore]
    public IEnumerable<IEntity> UncommittedEntities => _uncommittedEntities.AsReadOnly();
    protected readonly List<IEntity> _uncommittedEntities = [];

    protected void AddEvent(IDomainEvent @event)
    {
        _uncommittedEvents.Add(@event);
        ApplyEvent(@event);
        AddEntities(@event);
        Version++;
    }
    
    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
            Version++;
        }
    }
    
    protected abstract void ApplyEvent<T>(T @event) where T : IDomainEvent;
    protected abstract void AddEntities<T>(T @event) where T : IDomainEvent;
}
