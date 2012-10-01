using System;
using System.Collections.Concurrent;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Stores all commands in a concurrent queue
    /// </summary>
    public class MemoryStorage : ICommandStorage
    {
        private readonly ConcurrentQueue<CommandState> _queue = new ConcurrentQueue<CommandState>();

        #region ICommandStorage Members

        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        public void Enqueue(CommandState command)
        {
            if (command == null) throw new ArgumentNullException("command");
            _queue.Enqueue(command);
        }

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public CommandState Dequeue()
        {
            CommandState state;
            return !_queue.TryDequeue(out state) ? null : state;
        }

        #endregion
    }
}