namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Subscribes on a domain event
    /// </summary>
    /// <typeparam name="T">Domain event type</typeparam>
    /// <remarks>
    /// Read the interface name as <c>I Subscribe On "TheEventName"</c>.
    /// </remarks>
    public interface ISubscribeOn<in T> where T : class, IDomainEvent
    {
        /// <summary>
        /// Will be invoked when the domain event is triggered.
        /// </summary>
        /// <param name="domainEvent">Domin event to handle</param>
        void Handle(T domainEvent);
    }
}