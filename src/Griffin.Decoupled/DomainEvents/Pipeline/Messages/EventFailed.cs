using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    /// <summary>
    /// Failed to deliver a domain event.
    /// </summary>
    public class EventFailed
    {
        private readonly DispatchDomainEvent _failedEvent;
        private readonly Exception _exception;

        public EventFailed(DispatchDomainEvent failedEvent, Exception exception)
        {
            if (failedEvent == null) throw new ArgumentNullException("failedEvent");
            if (exception == null) throw new ArgumentNullException("exception");
            _failedEvent = failedEvent;
            _exception = exception;
        }

        /// <summary>
        /// Gets event which we failed to deliver
        /// </summary>
        public DispatchDomainEvent FailedEvent
        {
            get { return _failedEvent; }
        }

        /// <summary>
        /// Exception that was thrown.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
