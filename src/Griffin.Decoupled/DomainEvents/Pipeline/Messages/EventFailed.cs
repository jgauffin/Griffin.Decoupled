using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    /// <summary>
    /// Failed to deliver a domain event.
    /// </summary>
    public class EventFailed : IUpstreamMessage
    {
        private readonly Exception _exception;
        private readonly DispatchEvent _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventFailed" /> class.
        /// </summary>
        /// <param name="message">Message that failed.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.ArgumentNullException">failedEvent</exception>
        public EventFailed(DispatchEvent message, Exception exception)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (exception == null) throw new ArgumentNullException("exception");
            _message = message;
            _exception = exception;
        }

        /// <summary>
        /// Gets event which we failed to deliver
        /// </summary>
        public DispatchEvent Message
        {
            get { return _message; }
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