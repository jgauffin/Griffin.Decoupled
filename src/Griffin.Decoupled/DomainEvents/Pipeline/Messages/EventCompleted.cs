using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    /// <summary>
    /// The event has been dispatched successfully
    /// </summary>
    public class EventCompleted
    {
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
