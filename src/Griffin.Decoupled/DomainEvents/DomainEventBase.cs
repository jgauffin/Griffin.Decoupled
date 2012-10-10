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
            EventId = Guid.NewGuid();
        }

        #region IDomainEvent Members

        /// <summary>
        /// Gets domain event ID
        /// </summary>
        public Guid EventId { get; private set; }

        #endregion
    }
}