namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Implement this interface 
    /// </summary>
    public interface IUnitOfWorkAdapter
    {
        /// <summary>
        /// Register our own observer which is used to control when the domain events are dispatched.
        /// </summary>
        /// <param name="observer">Our observer.</param>
        void Register(IUnitOfWorkObserver observer);
    }
}