using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// Event arguments for <see cref="RetryingDispatcher.CommandFailed"/>.
    /// </summary>
    public class CommandFailed
    {
        private readonly SendCommand _state;
        private readonly Exception _exception;

        public CommandFailed(SendCommand state, Exception exception)
        {
            if (state == null) throw new ArgumentNullException("state");
            if (exception == null) throw new ArgumentNullException("exception");

            _state = state;
            _exception = exception;
        }

        /// <summary>
        /// Gets number of attempts that we've tried.
        /// </summary>
        public int NumberOfAttempts
        {
            get { return _state.Attempts; }
        }

        /// <summary>
        /// Gets failed command
        /// </summary>
        public ICommand Command { get { return _state.Command; } }

        /// <summary>
        /// Gets failed exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }
}