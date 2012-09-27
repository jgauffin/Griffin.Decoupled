using System;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Base class for domain events
    /// </summary>
    public class DomainEventBase : IDomainEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventBase" /> class.
        /// </summary>
        public DomainEventBase()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets domain event ID
        /// </summary>
        public Guid Id { get; private set; }
    }
}