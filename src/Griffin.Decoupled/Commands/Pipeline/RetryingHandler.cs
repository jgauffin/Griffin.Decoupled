using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands.Pipeline
{
    class RetryingHandler : IDownstreamHandler
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
        public RetryingHandler(int numberOfAttempts, ICommandStorage storage)
        {
            if (numberOfAttempts <= 0 || numberOfAttempts > 10)
                throw new ArgumentOutOfRangeException("numberOfAttempts", numberOfAttempts,
                                                      "1-10 attempts is more reasonable.");

            _numberOfAttempts = numberOfAttempts;
            _storage = storage;
        }


        public void HandleDownstream(IDownstreamContext context, object message)
        {
            var command = message as SendCommand;
            if (command != null)
            {
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
                        context.SendUpstream(new CommandFailed(command, err));
                    }
                    else
                    {
                        _storage.Enqueue(command);
                    }
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

    }
}
