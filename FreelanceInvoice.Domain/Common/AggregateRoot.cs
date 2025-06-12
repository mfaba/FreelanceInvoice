using System;
using System.Collections.Generic;
using System.Linq;

namespace FreelanceInvoice.Domain.Common;

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    void IAggregateRoot.AddDomainEvent(IDomainEvent domainEvent)
    {
        throw new NotImplementedException();
    }
} 