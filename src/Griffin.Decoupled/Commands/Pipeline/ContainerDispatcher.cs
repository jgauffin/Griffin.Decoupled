using System.Reflection;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Uses an inversion of control container to dispatch the messages.
    /// </summary>
    /// <remarks>
    /// A scope (child container) is created each time a new command should be invoked.
    /// </remarks>
    public class ContainerDispatcher : IDownstreamHandler
    {
        private readonly MethodInfo _method;
        private readonly IRootContainer _rootContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerDispatcher" /> class.
        /// </summary>
        /// <param name="rootContainer">The IoC container.</param>
        public ContainerDispatcher(IRootContainer rootContainer)
        {
            _rootContainer = rootContainer;
            _method = GetType().GetMethod("Dispatch", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="SendCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            var cmd = message as SendCommand;
            if (cmd != null)
            {
                _method.MakeGenericMethod(cmd.Command.GetType()).Invoke(this, new object[] {cmd.Command});
                return;
            }

            context.SendUpstream(
                new PipelineFailure(this,
                    "We, ContainerDispatcher, should be the last downstream handler. Received: " + message, null));
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