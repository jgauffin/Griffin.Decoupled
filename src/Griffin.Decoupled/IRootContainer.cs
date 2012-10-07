namespace Griffin.Decoupled
{
    /// <summary>
    /// Facade for an inversion of control container.
    /// </summary>
    /// <remarks>This facade is intended to be an adapter for the IoC handlers which exists in both the Command and DomainEvent namespace. Do note that both
    /// handlers will create a new scope for every command/event which is dispatched.</remarks>
    public interface IRootContainer : IServiceLocator
    {
        /// <summary>
        /// Create a new child scope.
        /// </summary>
        /// <returns>A new child scope</returns>
        IScopedContainer CreateScope();
    }
}