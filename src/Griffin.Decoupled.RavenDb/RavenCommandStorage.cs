using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Used to save all commands inside a RavenDb database.
    /// </summary>
    public class RavenCommandStorage : ICommandStorage 
    {
        private readonly IDocumentSession _session;

        public RavenCommandStorage(IDocumentSession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _session = session;
        }

        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        public void Enqueue(CommandState command)
        {
            _session.Store(command);
            _session.SaveChanges();
        }

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public CommandState Dequeue()
        {
            return _session.Query<CommandState>().FirstOrDefault();
        }
    }
}
