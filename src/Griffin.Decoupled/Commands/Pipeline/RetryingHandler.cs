using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Will try commands the configured amount of times before giving up.
    /// </summary>
    /// <remarks>
    /// The <see cref="CommandFailed"/> message will be sent upstream each time a command failes and finally the <see cref="CommandAborted"/> message
    /// when we give up on a command.
    /// </remarks>
    public class RetryingHandler : IDownstreamHandler
    {
        private readonly int _numberOfAttempts;
        private readonly ICommandStorage _storage;


        /// <summary>
        /// Initializes a new instance of the <see cref="RetryingHandler" /> class.
        /// </summary>
        /// <param name="numberOfAttempts">The number of attempts.</param>
        /// <param name="storage">Used to store failed commands (to retry later)</param>
        public RetryingHandler(int numberOfAttempts, ICommandStorage storage)
        {
            if (numberOfAttempts <= 0 || numberOfAttempts > 10)
                throw new ArgumentOutOfRangeException("numberOfAttempts", numberOfAttempts,
                                                      "1-10 attempts is more reasonable.");

            _numberOfAttempts = numberOfAttempts;
            _storage = storage;
        }

        #region IDownstreamHandler Members

        public void HandleDownstream(IDownstreamContext context, object message)
        {
            var command = message as SendCommand;
            if (command != null)
            {
                try
                {
                    context.SendDownstream(command);
                }
                catch (Exception err)
                {
                    command.Attempts++;
                    command.LastException = err.ToString();
                    if (command.Attempts >= _numberOfAttempts)
                    {
                        context.SendUpstream(new CommandAborted(command, err));
                    }
                    else
                    {
                        context.SendUpstream(new CommandFailed(command, err));
                        _storage.Enqueue(command);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
        }
    }
}