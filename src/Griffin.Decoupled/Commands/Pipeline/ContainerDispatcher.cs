using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IRootContainer _rootContainer;
        private readonly MethodInfo _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerDispatcher" /> class.
        /// </summary>
        /// <param name="rootContainer">The IoC container.</param>
        public ContainerDispatcher(IRootContainer rootContainer)
        {
            _rootContainer = rootContainer;
            _method = GetType().GetMethod("Dispatch", BindingFlags.Instance | BindingFlags.NonPublic);
        }

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
                _method.Invoke(this, new object[] {cmd});
                return;
            }

            context.SendUpstream(new PipelineError("We, ContainerDispatcher, should be the last downstream handler. Received: " + message, null));
        }

        /// <summary>
        /// Invoked through reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        private void Dispatch<T>(T command)  where T : class, ICommand
        {
            using(var scope = _rootContainer.CreateScope())
            {
                _rootContainer.Resolve<IHandleCommand<T>>().Invoke(command);
            }
        }
    }
}
