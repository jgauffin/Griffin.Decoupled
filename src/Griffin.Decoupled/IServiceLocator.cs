using System.Collections.Generic;

namespace Griffin.Decoupled
{
    /// <summary>
    /// Defines the actual service location
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Resolve all implementations
        /// </summary>
        /// <typeparam name="T">Service that we want implementations for.</typeparam>
        /// <returns>A collection of implementations; an empty collection if none is found.</returns>
        IEnumerable<T> ResolveAll<T>() where T : class;

        /// <summary>
        /// Get a specific service
        /// </summary>
        /// <typeparam name="T">Service to find</typeparam>
        /// <returns>Implementation</returns>
        T Resolve<T>() where T : class;
    }
}