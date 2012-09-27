using System;
using System.Collections.Generic;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Used to store domain events in a temporary storage
    /// </summary>
    /// <remarks>
    /// Storage is used to be able to dispatch events at a later point.
    /// </remarks>
    public interface IDomainEventStorage
    {
        /// <summary>
        /// Store a domain event
        /// </summary>
        /// <param name="batchId">Key used to store the domain event. It's not unique and therefore not PK either.</param>
        /// <param name="domainEvent">The actual domain event</param>
        void Store(Guid batchId, IDomainEvent domainEvent);

        /// <summary>
        /// Load all domain events which has been associated with a batch id.
        /// </summary>
        /// <param name="batchId">Id used when storing events</param>
        /// <returns>Collection of stored domain events (0..n). Empty collection if none was stored.</returns>
        IEnumerable<IDomainEvent> Load(Guid batchId);

        /// <summary>
        /// Delete all events which has been stored for the specified id
        /// </summary>
        /// <param name="batchId">Batch id</param>
        void Delete(Guid batchId);
    }
}