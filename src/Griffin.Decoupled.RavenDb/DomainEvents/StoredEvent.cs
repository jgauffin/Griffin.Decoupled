using System;
using System.Collections.Generic;
using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.RavenDb.DomainEvents
{
    /// <summary>
    /// A batch of domain events (model  used to store all domain events)
    /// </summary>
    public class Batch
    {
        private ICollection<IDomainEvent> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="Batch" /> class.
        /// </summary>
        /// <param name="batchId">The batch id.</param>
        public Batch(Guid batchId)
        {
            Id = batchId.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Batch" /> class.
        /// </summary>
        public Batch()
        {
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public ICollection<IDomainEvent> Events
        {
            get { return _events ?? (_events = new List<IDomainEvent>()); }
            set { _events = value; }
        }
    }
}