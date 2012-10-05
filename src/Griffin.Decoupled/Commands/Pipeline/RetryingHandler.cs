using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Listens on the upstream message <see cref="CommandFailed"/> and will add the command into the storage again on failure.
    /// </summary>
    /// <remarks>
    /// Will send the <see cref="CommandAborted"/> message upstream 
    /// when we give up on a command.
    /// </remarks>
    public class RetryingHandler : IUpstreamHandler
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

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
        }

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, object message)
        {
            var msg = message as CommandFailed;
            if (msg != null)
            {
                if (msg.Message.Attempts >= _numberOfAttempts)
                {
                    context.SendUpstream(new CommandAborted(msg.Message, msg.Exception));
                    _storage.Delete(msg.Message.Command);
                    return;
                }
                
                _storage.Update(msg.Message);
            }

            context.SendUpstream(message);
            
        }
    }
}