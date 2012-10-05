using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents.Pipeline
{
    /// <summary>
    /// Holds messages until their transaction has been committed.
    /// </summary>
    public class TransactionalHandler : IDownstreamHandler
    {
        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="SendCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            
        }
    }
}
