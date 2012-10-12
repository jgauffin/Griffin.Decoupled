using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    /// <summary>
    /// The event has been dispatched successfully
    /// </summary>
    public class EventCompleted : IUpstreamMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventCompleted" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public EventCompleted(DispatchEvent message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Message = message;
        }

        /// <summary>
        /// Gets the dispatch message for the event which has been completed.
        /// </summary>
        public DispatchEvent Message { get; private set; }
    }
}