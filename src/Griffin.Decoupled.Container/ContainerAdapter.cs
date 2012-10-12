using System;
using System.Collections.Generic;
using Griffin.Container;

namespace Griffin.Decoupled.Container
{
    /// <summary>
    /// Adapter for Griffin.Container
    /// </summary>
    public class ContainerAdapter : IRootContainer
    {
        private readonly IParentContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerAdapter" /> class.
        /// </summary>
        /// <param name="container">The Griffin.Container.</param>
        /// <exception cref="System.ArgumentNullException">container</exception>
        public ContainerAdapter(IParentContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        #region IRootContainer Members

        /// <summary>
        /// Resolve all implementations
        /// </summary>
        /// <typeparam name="T">Service that we want implementations for.</typeparam>
        /// <returns>A collection of implementations; an empty collection if none is found.</returns>
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return _container.ResolveAll<T>();
        }

        /// <summary>
        /// Get a specific service
        /// </summary>
        /// <typeparam name="T">Service to find</typeparam>
        /// <returns>Implementation</returns>
        public T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Create a new child scope.
        /// </summary>
        /// <returns>A new child scope</returns>
        public IScopedContainer CreateScope()
        {
            return new ChildContainerAdapter(_container.CreateChildContainer());
        }

        #endregion
    }
}