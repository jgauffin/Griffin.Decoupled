using System;
using Griffin.Decoupled.DomainEvents;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// RavenDb unit of work
    /// </summary>
    /// <remarks>
    /// <para>Griffin.Decoupled integration: Will invoke the <see cref="IUnitOfWorkObserver"/> for each UoW step.</para>
    /// <para>Raven integration: Will call <c>SaveChanges()</c> on commit and do nothing on dispose.</para></remarks>
    public class RavenDbUnitOfWork : IUnitOfWork
    {
        private readonly IUnitOfWorkObserver _observer;
        private bool _saved;

        public RavenDbUnitOfWork(IDocumentSession session, IUnitOfWorkObserver observer)
        {
            if (session == null) throw new ArgumentNullException("session");
            Session = session;
            _observer = observer;
            _observer.Create(this);
        }

        /// <summary>
        /// Gets the document session
        /// </summary>
        public IDocumentSession Session { get; private set; }

        #region IUnitOfWork Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (Session == null)
                return;

            Session = null;
            // do nothing. Let the container clean up the session
            if (!_saved)
                _observer.Released(this, false);
        }

        public void SaveChanges()
        {
            Session.SaveChanges();
            _saved = true;
            _observer.Released(this, true);
        }

        #endregion
    }
}