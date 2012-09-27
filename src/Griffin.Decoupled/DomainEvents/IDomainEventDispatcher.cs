using System;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Contract for the class which will dispatch the domain events.
    /// </summary>
    /// <remarks>Implementors should not handle exceptions that is thrown from any of the handlers. The outcome is undefind if a handler
    /// is throwing an exception. It should therefore abort the event handling. The framework will deliver these failures
    /// to the defined destination.</remarks>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatch domain event.
        /// </summary>
        /// <typeparam name="T">Domain event type</typeparam>
        /// <param name="domainEvent">The domain event</param>
        void Dispatch<T>(T domainEvent) where T : class, IDomainEvent;

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all events are propagated before returning.</remarks>
        void Close();
    }

}