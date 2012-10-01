using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Args for <see cref="AsyncCommandDispatcher.UncaughtException"/>.
    /// </summary>
    public class AsyncDispatcherExceptionEventArgs : EventArgs
    {
        public AsyncDispatcherExceptionEventArgs(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");

            Exception = exception;
        }

        /// <summary>
        /// Gets thrown exception
        /// </summary>
        public Exception Exception { get; private set; }
    }
}