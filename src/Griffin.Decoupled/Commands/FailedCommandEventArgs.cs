using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Event arguments for <see cref="RetryingDispatcher.CommandFailed"/>.
    /// </summary>
    public class FailedCommandEventArgs : EventArgs
    {
        private readonly CommandState _state;
        private readonly Exception _exception;
        private readonly ICommandStorage _storage;

        public FailedCommandEventArgs(CommandState state, Exception exception, ICommandStorage storage)
        {
            if (state == null) throw new ArgumentNullException("state");
            if (exception == null) throw new ArgumentNullException("exception");
            if (storage == null) throw new ArgumentNullException("storage");

            _state = state;
            _exception = exception;
            _storage = storage;
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
        public ICommand Command{get { return _state.Command; }}

        /// <summary>
        /// Gets failed exception
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Store command for another attempt.
        /// </summary>
        public void TryAgain()
        {
            _storage.Enqueue(_state);
        }
    }
}