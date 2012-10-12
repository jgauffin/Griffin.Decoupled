using System;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// We failed to deliver a command once, will however retry a few times more. So don't do anything rash just yet.
    /// </summary>
    public class CommandFailed : IUpstreamMessage
    {
        private readonly Exception _exception;
        private readonly DispatchCommand _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFailed" /> class.
        /// </summary>
        /// <param name="message">Message which failed.</param>
        /// <param name="exception">The exception that cause the dispatching to fail.</param>
        /// <exception cref="System.ArgumentNullException">The arguments are required</exception>
        public CommandFailed(DispatchCommand message, Exception exception)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (exception == null) throw new ArgumentNullException("exception");

            _message = message;
            _exception = exception;
        }

        /// <summary>
        /// Gets number of attempts that we've tried.
        /// </summary>
        public int NumberOfAttempts
        {
            get { return _message.Attempts; }
        }

        /// <summary>
        /// Gets failed command
        /// </summary>
        public DispatchCommand Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Gets failed exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Command '{0}' failed, Attempt '{1}' \r\n{2}", _message, NumberOfAttempts, Exception);
        }
    }
}