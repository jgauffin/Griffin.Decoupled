using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// A command could not be delivered and we'll therefore give up on it.
    /// </summary>
    public class CommandAborted
    {
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