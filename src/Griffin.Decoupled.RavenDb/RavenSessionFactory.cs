using System;
using Griffin.Decoupled.DomainEvents;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Implements the required interfaces for allowing the 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class RavenSessionFactory : IUnitOfWorkAdapter
    {
        private readonly IDocumentStore _documentStore;
        private IUnitOfWorkObserver _observer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenSessionFactory" /> class.
        /// </summary>
        /// <param name="documentStore">The document store.</param>
        /// <exception cref="System.ArgumentNullException">documentStore</exception>
        public RavenSessionFactory(IDocumentStore documentStore)
        {
            if (documentStore == null) throw new ArgumentNullException("documentStore");
            _documentStore = documentStore;
        }

        #region IUnitOfWorkAdapter Members

        /// <summary>
        /// Register our own observer which is used to control when the domain events are dispatched.
        /// </summary>
        /// <param name="observer">Our observer.</param>
        public void Register(IUnitOfWorkObserver observer)
        {
            if (observer == null) throw new ArgumentNullException("observer");

            _observer = observer;
        }

        #endregion

        /// <summary>
        /// Create a new unit of work
        /// </summary>
        /// <returns></returns>
        public RavenDbUnitOfWork CreateUnitOfWork()
        {
            return new RavenDbUnitOfWork(_documentStore.OpenSession(), _observer);
        }
    }
}