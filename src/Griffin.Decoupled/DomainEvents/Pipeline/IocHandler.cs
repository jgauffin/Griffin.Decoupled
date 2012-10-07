using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents.Pipeline
{
    /// <summary>
    /// Will dispatch messages to all subscribers.
    /// </summary>
    /// <remarks>Should be the last downstream handler in the pipeline (will not pass any messages on).</remarks>
    public class IocHandler : IDownstreamHandler
    {
        private readonly IRootContainer _rootContainer;
        private readonly MethodInfo _method;
        private IDownstreamContext _context;

        public IocHandler(IRootContainer rootContainer)
        {
            _rootContainer = rootContainer;
            _method = GetType().GetMethod("Dispatch", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Send a message to the domainEvent handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="SendCommand"/>.</param>
        public virtual void HandleDownstream(IDownstreamContext context, object message)
        {
            _context = context;
            var dispatchMsg = message as DispatchEvent;
            if (dispatchMsg != null)
            {
                try
                {
                    _method.MakeGenericMethod(dispatchMsg.DomainEvent.GetType()).Invoke(this, new object[] { dispatchMsg.DomainEvent });    
                    return;
                }
                catch(Exception err)
                {
                    context.SendUpstream(new EventFailed(dispatchMsg, err));
                }
            }
        }

        /// <summary>
        /// Invoked through reflection
        /// </summary>
        /// <typeparam name="T">Type of event</typeparam>
        /// <param name="domainEvent">The event that should be dispatched</param>
        private void Dispatch<T>(T domainEvent) where T : class, IDomainEvent
        {
            using (var scope = _rootContainer.CreateScope())
            {
                foreach (var subscriber in scope.ResolveAll<ISubscribeOn<T>>())
                {
                    subscriber.Handle(domainEvent);
                }
            }
        }

    }
}
