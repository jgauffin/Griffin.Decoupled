using System;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Used when a dispatcher fails to handle a command. (Usually means that a subscriber have failed).
    /// </summary>
    public class DispatcherFailedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherFailedEventArgs" /> class.
        /// </summary>
        /// <param name="exception">The thrown exception.</param>
        /// <param name="domainEvent">The domain event in trouble.</param>
        public DispatcherFailedEventArgs(Exception exception, IDomainEvent domainEvent)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            if (domainEvent == null) throw new ArgumentNullException("domainEvent");

            Exception = exception;
            DomainEvent = domainEvent;
        }

        /// <summary>
        /// Gets thrown exception
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets domain event in trouble.
        /// </summary>
        public IDomainEvent DomainEvent { get; private set; }
    }
}