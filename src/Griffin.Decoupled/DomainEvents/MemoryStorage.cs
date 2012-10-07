using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Stores domain events in memory.
    /// </summary>
    public class MemoryStorage : IDomainEventStorage
    {
        private readonly ConcurrentDictionary<Guid, ICollection<IDomainEvent>> _domainEvents =
            new ConcurrentDictionary<Guid, ICollection<IDomainEvent>>();

        #region IDomainEventStorage Members

        /// <summary>
        /// Store a domain event
        /// </summary>
        /// <param name="batchId">Key used to store the domain event. It's not unique and therefore not PK either.</param>
        /// <param name="domainEvent">The actual domain event</param>
        /// <exception cref="System.ArgumentException">Batchid is not specified.</exception>
        public void Hold(Guid batchId, IDomainEvent domainEvent)
        {
            if (batchId == Guid.Empty)
                throw new ArgumentException("You must specify a batchId", "batchId");

            var domainEvents = _domainEvents.GetOrAdd(batchId, x => new LinkedList<IDomainEvent>());

            // the actual list should only be used on the same thread
            // hence thread safe.
            domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Load all domain events which has been associated with a batch id.
        /// </summary>
        /// <param name="batchId">Id used when storing events</param>
        /// <returns>Collection of stored domain events (0..n). Empty collection if none was stored.</returns>
        public IEnumerable<IDomainEvent> Release(Guid batchId)
        {
            if (batchId == Guid.Empty)
                throw new ArgumentException("You must specify a batchId", "batchId");

            ICollection<IDomainEvent> domainEvents;
            return _domainEvents.TryGetValue(batchId, out domainEvents) ? domainEvents : new IDomainEvent[0];
        }


        /// <summary>
        /// Delete all events which has been stored for the specified id
        /// </summary>
        /// <param name="batchId">Batch id</param>
        public void Delete(Guid batchId)
        {
            if (batchId == Guid.Empty)
                throw new ArgumentException("You must specify a batchId", "batchId");

            ICollection<IDomainEvent> domainEvents;
            if (!_domainEvents.TryRemove(batchId, out domainEvents))
                if (!_domainEvents.TryRemove(batchId, out domainEvents))
                    throw new InvalidOperationException(string.Format("Failed to remove events for batch '{0}'.",
                                                                      batchId));
        }

        #endregion
    }
}