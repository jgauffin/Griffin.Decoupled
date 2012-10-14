using System;
using System.Collections.Generic;
using Griffin.Container;

namespace Griffin.Decoupled.Container
{
    /// <summary>
    /// Adapter for the scoped container.
    /// </summary>
    public class ChildContainerAdapter : IScopedContainer
    {
        private IChildContainer _childContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildContainerAdapter" /> class.
        /// </summary>
        /// <param name="childContainer">The child container (i.e. scoped container which will clean up scoped services when being disposed).</param>
        /// <exception cref="System.ArgumentNullException">childContainer</exception>
        public ChildContainerAdapter(IChildContainer childContainer)
        {
            if (childContainer == null) throw new ArgumentNullException("childContainer");
            _childContainer = childContainer;
        }

        #region IScopedContainer Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_childContainer == null)
                return;

            _childContainer.Dispose();
            _childContainer = null;
        }

        /// <summary>
        /// Resolve all implementations
        /// </summary>
        /// <typeparam name="T">Service that we want implementations for.</typeparam>
        /// <returns>A collection of implementations; an empty collection if none is found.</returns>
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return _childContainer.ResolveAll<T>();
        }

        /// <summary>
        /// Get a specific service
        /// </summary>
        /// <typeparam name="T">Service to find</typeparam>
        /// <returns>Implementation</returns>
        public T Resolve<T>() where T : class
        {
            return _childContainer.Resolve<T>();
        }

        /// <summary>
        /// Get a specific service
        /// </summary>
        /// <param name="type">Service to find</param>
        /// <returns>Implementation</returns>
        public object Resolve(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return _childContainer.Resolve(type);
        }

        #endregion
    }
}