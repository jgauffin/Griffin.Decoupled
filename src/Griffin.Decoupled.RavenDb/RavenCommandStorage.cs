using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
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
        public void Add(SendCommand command)
        {
            _session.Store(new StoredCommand(command));
            _session.SaveChanges();
        }

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public SendCommand Dequeue()
        {
            var cmd = _session.Query<StoredCommand>().FirstOrDefault();
            if (cmd == null)
                return null;

            cmd.ProcessedAt = DateTime.Now;
            _session.Store(cmd);
            _session.SaveChanges();
            return cmd.Command;
        }

        /// <summary>
        /// Re add a command which we've tried to invoke but failed.
        /// </summary>
        /// <param name="command">Command to add</param>
        public void Update(SendCommand command)
        {
            var cmd = _session.Load<StoredCommand>(command.Command.Id);
            if (cmd == null)
                return;

            cmd.Command = command;
            _session.Store(command);
            _session.SaveChanges();
        }

        /// <summary>
        /// Delete a command
        /// </summary>
        /// <param name="command">Command to delete from storage</param>
        public void Delete(ICommand command)
        {
            var cmd = _session.Load<StoredCommand>(command.Id);
            if (cmd == null)
                return;

            _session.Delete(cmd);
            _session.SaveChanges();
        }

        /// <summary>
        /// Find commands which has been marked as processed but not deleted.
        /// </summary>
        /// <param name="markedAsProcessBefore">Get all commands that were marked as being processed before this date/time.</param>
        /// <returns>Any matching commands or an empty collection.</returns>
        public IEnumerable<SendCommand> FindFailedCommands(DateTime markedAsProcessBefore)
        {
            return
                _session.Query<StoredCommand>().Where(x => x.ProcessedAt < markedAsProcessBefore).Select(x => x.Command);
        }
    }
}
