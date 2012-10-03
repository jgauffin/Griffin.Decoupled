using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    /// <summary>
    /// Stores all commands in a concurrent queue
    /// </summary>
    public class TestStorage : ICommandStorage
    {
        private readonly ConcurrentQueue<SendCommand> _queue = new ConcurrentQueue<SendCommand>();
        private readonly List<SendCommand> _dequeued = new List<SendCommand>();
        public IEnumerable<SendCommand> StoredItems
        {
            get { return _queue; }
        }

        public List<SendCommand> Dequeued
        {
            get { return _dequeued; }
        }

        #region ICommandStorage Members

        /// <summary>
        /// Enqueue a command
        /// </summary>
        /// <param name="command">Get the command which was </param>
        public void Enqueue(SendCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            _queue.Enqueue(command);
        }

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public SendCommand Dequeue()
        {
            SendCommand state;
            if (!_queue.TryDequeue(out state))
            return null;
            Dequeued.Add(state);
            
            return state;
        }

        #endregion
    }
}
