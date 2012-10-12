using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// A command could not be delivered and we'll therefore give up on it.
    /// </summary>
    public class CommandAborted : IUpstreamMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAborted" /> class.
        /// </summary>
        /// <param name="message">The message that was aborted.</param>
        /// <param name="exception">The exception which aborted the dispatching.</param>
        /// <exception cref="System.ArgumentNullException">The fields are required</exception>
        public CommandAborted(DispatchCommand message, Exception exception)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (exception == null) throw new ArgumentNullException("exception");
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// Gets the dispatch method for the command in question.
        /// </summary>
        public DispatchCommand Message { get; private set; }

        /// <summary>
        /// Gets last exception which made us abort the handling
        /// </summary>
        public Exception Exception { get; private set; }
    }
}