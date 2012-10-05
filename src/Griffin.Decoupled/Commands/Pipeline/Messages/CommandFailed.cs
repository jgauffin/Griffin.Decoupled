using System;

namespace Griffin.Decoupled.Commands.Pipeline.Messages
{
    /// <summary>
    /// We failed to deliver a command once, will however retry a few times more. So don't do anything rash just yet.
    /// </summary>
    public class CommandFailed
    {
        private readonly Exception _exception;
        private readonly SendCommand _state;

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
        public SendCommand Message
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets failed exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }
}