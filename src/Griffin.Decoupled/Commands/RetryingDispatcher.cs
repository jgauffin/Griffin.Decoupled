using System;
using System.Threading;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// A dispatcher that will retry a command if it fails.
    /// </summary>
    /// <remarks>The dispatcher will pause the thread a tiny bit before retrying.</remarks>
    public class RetryingDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher _inner;
        private readonly int _numberOfAttempts;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryingDispatcher" /> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="numberOfAttempts">The number of attempts.</param>
        public RetryingDispatcher(ICommandDispatcher inner, int numberOfAttempts)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (numberOfAttempts <= 0 || numberOfAttempts > 10)
                throw new ArgumentOutOfRangeException("numberOfAttempts", numberOfAttempts,
                                                      "1-10 attempts is more reasonable.");

            _inner = inner;
            _numberOfAttempts = numberOfAttempts;
        }

        #region ICommandDispatcher Members

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch<T>(T command) where T : class, ICommand
        {
            if (command == null) throw new ArgumentNullException("command");

            var attempts = 0;
            while (true)
            {
                try
                {
                    _inner.Dispatch(command);
                }
                catch (Exception)
                {
                    attempts++;
                    if (attempts <= _numberOfAttempts)
                        throw;

                    // Just wait a tiny bit before retrying
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
        }

        #endregion
    }
}