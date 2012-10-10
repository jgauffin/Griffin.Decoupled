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
    /// <para>Will dispatch the message <see cref="CommandCompleted"/> upstream if the invocation is successful or <see cref="CommandFailed"/>
    /// if the invocation failed.</para>
    /// </remarks>
    public class IocDispatcher : IDownstreamHandler
    {
        private readonly MethodInfo _method;
        private readonly IRootContainer _rootContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocDispatcher" /> class.
        /// </summary>
        /// <param name="rootContainer">The IoC container.</param>
        public IocDispatcher(IRootContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");
            _rootContainer = rootContainer;
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
                    context.SendUpstream(new CommandCompleted(msg));
                }
                catch (Exception err)
                {
                    msg.AddFailedAttempt();
                    context.SendUpstream(new CommandFailed(msg, err));
                    return;
                }

                return;
            }

            context.SendDownstream(message);
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