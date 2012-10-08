using System;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    /// <summary>
    /// Failed to deliver a domain event.
    /// </summary>
    public class EventFailed
    {
        private readonly Exception _exception;
        private readonly DispatchEvent _failedEvent;

        public EventFailed(DispatchEvent failedEvent, Exception exception)
        {
            if (failedEvent == null) throw new ArgumentNullException("failedEvent");
            if (exception == null) throw new ArgumentNullException("exception");
            _failedEvent = failedEvent;
            _exception = exception;
        }

        /// <summary>
        /// Gets event which we failed to deliver
        /// </summary>
        public DispatchEvent FailedEvent
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