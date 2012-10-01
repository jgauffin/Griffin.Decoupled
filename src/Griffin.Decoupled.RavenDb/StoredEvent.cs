using System;
using System.Collections.Generic;
using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// A batch of domain events (model  used to store all domain events)
    /// </summary>
    public class Batch
    {
        private ICollection<IDomainEvent> _events;

        public Batch(Guid batchId)
        {
            Id = batchId.ToString();
        }

        public Batch()
        {
        }

        public string Id { get; set; }

        public ICollection<IDomainEvent> Events
        {
            get { return _events ?? (_events = new List<IDomainEvent>()); }
            set { _events = value; }
        }
    }
}