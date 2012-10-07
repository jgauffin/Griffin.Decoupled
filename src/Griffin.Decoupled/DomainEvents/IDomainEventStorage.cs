using System;
using System.Collections.Generic;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Used to store domain events in a temporary storage
    /// </summary>
    /// <remarks>
    /// <para>
    /// Storage is used to be able to dispatch events at a later point. Each implementation should be thread safe
    /// since it will be invoked from all dispatcher threads. You are also reponsible of keeping the connection
    /// to the database open.
    /// </para>
    /// <para>Do note that it means that all domain events are removed when they have been processed, no matter
    /// if the processing was successful or not. If you would like to implement event source then simply
    /// create a new downstream handler and use that one to store all events. The playback can be made
    /// with the help of the <see cref="Started"/> message in the pipeline.</para>
    /// </remarks>
    public interface IDomainEventStorage
    {
        /// <summary>
        /// Store a domain event
        /// </summary>
        /// <param name="batchId">Key used to store the domain event. It's not unique and therefore not PK either.</param>
        /// <param name="domainEvent">The actual domain event</param>
        void Hold(Guid batchId, IDomainEvent domainEvent);

        /// <summary>
        /// Load all domain events which has been associated with a batch id.
        /// </summary>
        /// <param name="batchId">Id used when storing events</param>
        /// <returns>Collection of stored domain events (0..n). Empty collection if none was stored.</returns>
        IEnumerable<IDomainEvent> Release(Guid batchId);

        /// <summary>
        /// Delete all events which has been stored for the specified id
        /// </summary>
        /// <param name="batchId">Batch id</param>
        void Delete(Guid batchId);
    }
}