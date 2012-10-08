using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Used to abstract away the storage handling from the rest of the handlers.
    /// </summary>
    public class StorageHandler : IDownstreamHandler, IUpstreamHandler
    {
        private readonly ICommandStorage _storage;

        public StorageHandler(ICommandStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            var msg = message as DispatchCommand;
            if (msg != null)
            {
                _storage.Add(msg);
                
            }
        }

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, object message)
        {
            throw new NotImplementedException();
        }
    }
}
