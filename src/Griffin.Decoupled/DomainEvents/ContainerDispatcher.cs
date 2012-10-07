using System;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Uses an inversion of control container to dispatch the events synchronously..
    /// </summary>
    /// <remarks>You need to implement the <see cref="IRootContainer"/> interface to be able to use this class.</remarks>
    public class ContainerDispatcher : IDomainEventDispatcher
    {
        private readonly IRootContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerDispatcher" /> class.
        /// </summary>
        /// <param name="container">The service locator (adapter for your favorite IoC container adapter).</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ContainerDispatcher(IRootContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
        }

        #region IDomainEventDispatcher Members

        /// <summary>
        /// Dispatch domain event.
        /// </summary>
        /// <typeparam name="T">Domain event type</typeparam>
        /// <param name="domainEvent">The domain event</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Dispatch<T>(T domainEvent) where T : class, IDomainEvent
        {
            if (domainEvent == null) throw new ArgumentNullException("domainEvent");

            using (var scope = _container.CreateScope())
            {
                foreach (var handler in scope.ResolveAll<ISubscribeOn<T>>())
                {
                    handler.Handle(domainEvent);
                }
            }
        }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all events are propagated before returning.</remarks>
        public void Close()
        {
        }

        #endregion
    }
}