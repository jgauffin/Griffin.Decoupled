using System;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Base interface for domain events
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets domain event ID
        /// </summary>
        Guid Id { get; }
    }
}