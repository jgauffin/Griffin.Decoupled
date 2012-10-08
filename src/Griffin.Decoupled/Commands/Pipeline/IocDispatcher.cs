using System;
using System.Reflection;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Uses an inversion of control container to dispatch the messages.
    /// </summary>
    /// <remarks>
    /// A scope (child container) is created each time a new command should be invoked.
    /// </remarks>
    public class IocDispatcher : IDownstreamHandler
    {
        private readonly MethodInfo _method;
        private readonly IRootContainer _rootContainer;
        private readonly ICommandStorage _storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocDispatcher" /> class.
        /// </summary>
        /// <param name="rootContainer">The IoC container.</param>
        /// <param name="storage">Used to delete commands when they have been successfully being executed.</param>
        public IocDispatcher(IRootContainer rootContainer, ICommandStorage storage)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");
            if (storage == null) throw new ArgumentNullException("storage");
            _rootContainer = rootContainer;
            _storage = storage;
            _method = GetType().GetMethod("Dispatch", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            var msg = message as DispatchCommand;
            if (msg != null)
            {
                try
                {
                    _method.MakeGenericMethod(msg.Command.GetType()).Invoke(this, new object[] {msg.Command});
                    _storage.Delete(msg.Command);
                }
                catch (Exception err)
                {
                    msg.AddFailedAttempt();
                    context.SendUpstream(new CommandFailed(msg, err));
                }
                return;
            }

            context.SendUpstream(
                new PipelineFailure(this,
                                    message,
                                    "We, IocDispatcher, should be the last downstream handler. Received: " + message,
                                    null));
        }

        #endregion

        /// <summary>
        /// Invoked through reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        private void Dispatch<T>(T command) where T : class, ICommand
        {
            using (var scope = _rootContainer.CreateScope())
            {
                scope.Resolve<IHandleCommand<T>>().Invoke(command);
            }
        }
    }
}