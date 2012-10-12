using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    /// <summary>
    /// Dispatch a domain event.
    /// </summary>
    public class DispatchEvent : IDownstreamMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchEvent" /> class.
        /// </summary>
        /// <param name="domainEvent">The domain event.</param>
        /// <exception cref="System.ArgumentNullException">domainEvent</exception>
        public DispatchEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null) throw new ArgumentNullException("domainEvent");
            DomainEvent = domainEvent;
        }

        /// <summary>
        /// Gets event to dispatch
        /// </summary>
        public IDomainEvent DomainEvent { get; private set; }
    }
}