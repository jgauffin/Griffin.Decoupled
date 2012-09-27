namespace Griffin.Decoupled
{
    /// <summary>
    /// Facade for an inversion of control container.
    /// </summary>
    public interface IRootContainer : IServiceLocator
    {
        /// <summary>
        /// Create a new child scope.
        /// </summary>
        /// <returns>A new child scope</returns>
        IScopedContainer CreateScope();
    }
}