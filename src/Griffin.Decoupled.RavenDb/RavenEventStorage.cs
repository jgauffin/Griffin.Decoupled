using System;
using System.Collections.Generic;
using Griffin.Decoupled.DomainEvents;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Uses ravenDb to store events.
    /// </summary>
    public class RavenEventStorage : IDomainEventStorage
    {
        private readonly IDocumentSession _session;

        public RavenEventStorage(IDocumentSession session)
        {
            _session = session;
        }

        #region IDomainEventStorage Members

        /// <summary>
        /// Store a domain event
        /// </summary>
        /// <param name="batchId">Key used to store the domain event. It's not unique and therefore not PK either.</param>
        /// <param name="domainEvent">The actual domain event</param>
        public void Store(Guid batchId, IDomainEvent domainEvent)
        {
            var batch = _session.Load<Batch>(batchId) ?? new Batch(batchId);
            batch.Events.Add(domainEvent);
            _session.Store(batch);
            _session.SaveChanges();
        }

        /// <summary>
        /// Load all domain events which has been associated with a batch id.
        /// </summary>
        /// <param name="batchId">Id used when storing events</param>
        /// <returns>Collection of stored domain events (0..n). Empty collection if none was stored.</returns>
        public IEnumerable<IDomainEvent> Load(Guid batchId)
        {
            var batch = _session.Load<Batch>(batchId);
            return batch != null ? batch.Events : new IDomainEvent[0];
        }

        /// <summary>
        /// Delete all events which has been stored for the specified id
        /// </summary>
        /// <param name="batchId">Batch id</param>
        public void Delete(Guid batchId)
        {
            var batch = _session.Load<Batch>(batchId);
            if (batch != null)
                _session.Delete(batch);
        }

        #endregion
    }
}