using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Singleton proxy for the domain event dispatching
    /// </summary>
    /// <remarks>The actual dispatching is made by a <see cref="IDomainEventDispatcher"/>. The recommended approach is to use <see cref="ContainerDispatcher"/>. An implementation
    /// is specified by using the <c>DomainEvent.Assign()</c> method.
    /// </remarks>
    public class DomainEvent
    {
        private static IDomainEventDispatcher _domainEventDispatcher;

        /// <summary>
        /// Assigns the specified domain event dispatcher.
        /// </summary>
        /// <param name="dispatcher">The domain event dispatcher.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Assign(IDomainEventDispatcher dispatcher)
        {
            _domainEventDispatcher = dispatcher;
        }

        /// <summary>
        /// Dispatch an domain event
        /// </summary>
        /// <typeparam name="T">Type of domain event</typeparam>
        /// <param name="domainEvent">Event to dispatch</param>
        /// <remarks>The domain event will either be dispatched synchronusly or async depending on the used implementation.</remarks>
        public static void Dispatch<T>(T domainEvent) where T : class, IDomainEvent
        {
            if (_domainEventDispatcher == null)
                throw new InvalidOperationException("A domain event dispatcher has not been specified. Read the class documentation for the DomainEvent class.");

            _domainEventDispatcher.Dispatch(domainEvent);
        }
    }
}
