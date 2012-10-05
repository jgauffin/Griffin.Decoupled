using System;
using System.Linq;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Will not invoke <c>SaveChanges()</c> until the transaction is commited.
    /// </summary>
    public class RavenDbTransactionalCommandStorage : ITransactionalCommandStorage
    {
        private readonly IDocumentSession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenDbTransactionalCommandStorage" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public RavenDbTransactionalCommandStorage(IDocumentSession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        public void Enqueue(SendCommand command)
        {
            _session.Store(command);
        }

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public SendCommand Dequeue()
        {
            return _session.Query<SendCommand>().FirstOrDefault();
        }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <returns>Newly created transaction</returns>
        public ISimpleTransaction BeginTransaction()
        {
            return new FakeTransaction(_session);
        }

        class FakeTransaction:ISimpleTransaction

        {
            private readonly IDocumentSession _session;

            public FakeTransaction(IDocumentSession session)
            {
                _session = session;
            }
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                _session.Dispose();
            }

            /// <summary>
            /// Commit transaction
            /// </summary>
            public void Commit()
            {
                _session.SaveChanges();
            }
        }
    }
}