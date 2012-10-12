using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// A command has been successfully processed.
    /// </summary>
    public class CommandCompleted : IUpstreamMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCompleted" /> class.
        /// </summary>
        /// <param name="message">Message for the command that was completed.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public CommandCompleted(DispatchCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Message = message;
        }

        /// <summary>
        /// Gets the dispatch message for the command which was completed.
        /// </summary>
        public DispatchCommand Message { get; private set; }
    }
}