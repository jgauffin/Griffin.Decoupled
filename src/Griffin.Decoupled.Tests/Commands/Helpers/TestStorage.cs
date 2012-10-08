using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    /// <summary>
    /// Stores all commands in a concurrent queue
    /// </summary>
    public class TestStorage : ICommandStorage
    {
        private readonly List<DispatchCommand> _dequeued = new List<DispatchCommand>();
        private readonly ConcurrentQueue<DispatchCommand> _queue = new ConcurrentQueue<DispatchCommand>();

        public IEnumerable<DispatchCommand> StoredItems
        {
            get { return _queue; }
        }

        public List<DispatchCommand> Dequeued
        {
            get { return _dequeued; }
        }

        #region ICommandStorage Members

        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        public void Add(DispatchCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            _queue.Enqueue(command);
        }

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public DispatchCommand Dequeue()
        {
            DispatchCommand state;
            if (!_queue.TryDequeue(out state))
                return null;
            Dequeued.Add(state);

            return state;
        }

        /// <summary>
        /// Re add a command which we've tried to invoke but failed.
        /// </summary>
        /// <param name="command">Command to add</param>
        public void Update(DispatchCommand command)
        {
        }

        /// <summary>
        /// Delete a command
        /// </summary>
        /// <param name="command">Command to delete from storage</param>
        public void Delete(ICommand command)
        {
        }

        /// <summary>
        /// Find commands which has been marked as processed but not deleted.
        /// </summary>
        /// <param name="markedAsProcessBefore">Get all commands that were marked as being processed before this date/time.</param>
        /// <returns>Any matching commands or an empty collection.</returns>
        public IEnumerable<DispatchCommand> FindFailedCommands(DateTime markedAsProcessBefore)
        {
            return null;
        }

        #endregion
    }
}