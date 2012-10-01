using System;
using System.Threading;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// A dispatcher that will retry a command if it fails.
    /// </summary>
    /// <remarks>This dispatcher will store all failed commands for later attempts. It will however not trigger those attempts.
    /// You have to do that. Keep in mind that all commands that have failed all times will be thrown away unless
    /// you have subscribed on the <see cref="CommandFailed"/> event.</remarks>
    public class RetryingDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher _inner;
        private readonly int _numberOfAttempts;
        private readonly ICommandStorage _storage;


        /// <summary>
        /// Initializes a new instance of the <see cref="RetryingDispatcher" /> class.
        /// </summary>
        /// <param name="inner">The inner.</param>
        /// <param name="numberOfAttempts">The number of attempts.</param>
        /// <param name="storage">Used to store failed commands (to retry later)</param>
        public RetryingDispatcher(ICommandDispatcher inner, int numberOfAttempts, ICommandStorage storage)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            if (numberOfAttempts <= 0 || numberOfAttempts > 10)
                throw new ArgumentOutOfRangeException("numberOfAttempts", numberOfAttempts,
                                                      "1-10 attempts is more reasonable.");

            _inner = inner;
            _numberOfAttempts = numberOfAttempts;
            _storage = storage;
        }

        #region ICommandDispatcher Members

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch(CommandState command)
        {
            if (command == null) throw new ArgumentNullException("command");

            try
            {
                _inner.Dispatch(command);
            }
            catch (Exception err)
            {
                command.Attempts++;
                command.LastException = err.ToString();
                if (command.Attempts >= _numberOfAttempts)
                {
                    CommandFailed(this, new FailedCommandEventArgs(command, err, _storage));
                }
                else
                {
                    _storage.Enqueue(command);
                }
            }
        }

        /// <summary>
        /// Invoked when we've tried a command several times and it's still failing
        /// </summary>
        public event EventHandler<FailedCommandEventArgs> CommandFailed = delegate { };

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