using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Raven.Client;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Used to save all commands inside a RavenDb database.
    /// </summary>
    /// <remarks>Will create a new session each time an operation is made.</remarks>
    public class RavenCommandStorage : ICommandStorage
    {
        private readonly IDocumentStore _documentStore;

        public RavenCommandStorage(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        #region ICommandStorage Members

        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        public void Add(DispatchCommand command)
        {
            using (var session = _documentStore.OpenSession())
            {
                Diagnostics(command.Command.CommandId, session, "Add");
                var cmd = new StoredCommand(command);
                cmd.ProcessedAt = DateTime.Now;
                session.Store(cmd);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Re add a command which we've tried to invoke but failed.
        /// </summary>
        /// <param name="command">Command to add</param>
        public void Update(DispatchCommand command)
        {
            using (var session = _documentStore.OpenSession())
            {
                var cmd = session.Load<StoredCommand>(command.Command.CommandId);
                if (cmd == null)
                    throw new InvalidOperationException("Failed to find command with id: " + command.Command.CommandId);

                cmd.Command = command.Command;
                cmd.Attempts = command.Attempts;
                cmd.ProcessedAt = null;
                session.Store(command);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Delete a command
        /// </summary>
        /// <param name="command">Command to delete from storage</param>
        public void Delete(ICommand command)
        {
            using (var session = _documentStore.OpenSession())
            {
                Diagnostics(command.CommandId, session, "Delete");

                var cmd = session.Load<StoredCommand>(command.CommandId);
                if (cmd == null)
                {
                    throw new InvalidOperationException("Failed to find command " + command.CommandId);
                }

                session.Delete(cmd);
                session.SaveChanges();
            }
        }

        /// <summary>
        /// Find commands which has been marked as processed but not deleted.
        /// </summary>
        /// <param name="markedAsProcessBefore">Get all commands that were marked as being processed before this date/time.</param>
        /// <returns>Any matching commands or an empty collection.</returns>
        public IEnumerable<DispatchCommand> FindFailedCommands(DateTime markedAsProcessBefore)
        {
            using (var session = _documentStore.OpenSession())
            {
                return
                    session.Query<StoredCommand>().Where(x => x.ProcessedAt < markedAsProcessBefore).Select(BuildMessage);
            }
        }

        #endregion

        private void Diagnostics(Guid id, IDocumentSession sessiom, string methodName)
        {
            return;
            Console.WriteLine(methodName + ": Working with " + id);
            foreach (var cmd in sessiom.Query<StoredCommand>())
            {
                Console.WriteLine(methodName + ": Existing: " + cmd.Id);
            }
        }

        private DispatchCommand BuildMessage(StoredCommand entity)
        {
            return new DispatchCommand(entity.Command, entity.Attempts);
        }
    }
}