namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Allows Griffin.Decoupled to monitor YOUR transactions to be able to release the domain events when they commit successfully.
    /// </summary>
    /// <remarks>This interface has nothing to do with the <see cref="IDomainEventStorage"/>.
    /// </remarks>
    public interface IUnitOfWorkAdapter
    {
        /// <summary>
        /// Register our own observer which is used to control when the domain events are dispatched.
        /// </summary>
        /// <param name="observer">Our observer.</param>
        void Register(IUnitOfWorkObserver observer);
    }
}