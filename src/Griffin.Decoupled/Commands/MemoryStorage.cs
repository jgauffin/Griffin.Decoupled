using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Stores all commands in a concurrent queue
    /// </summary>
    public class MemoryStorage : ICommandStorage
    {
        private readonly object _lock = new object();
        private readonly List<MyStruct> _queue = new List<MyStruct>();

        #region ICommandStorage Members

        /// <summary>
        /// Add a new command
        /// </summary>
        /// <param name="command">Store the command in the DB. You can use the <see cref="ICommand.CommandId"/> as an identity.</param>
        public void Add(DispatchCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            lock (_lock)
            {
                _queue.Add(new MyStruct {Command = command});
            }
        }

        /// <summary>
        /// Re add a command which we've tried to invoke but failed.
        /// </summary>
        /// <param name="command">Command to add</param>
        public void Update(DispatchCommand command)
        {
            lock (_lock)
            {
                var first = _queue.FirstOrDefault(x => x.Command.Command.CommandId == command.Command.CommandId);
                if (first != null)
                    first.StartedAt = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Delete a command
        /// </summary>
        /// <param name="command">Command to delete from storage</param>
        public void Delete(ICommand command)
        {
            lock (_queue)
            {
                _queue.RemoveAll(x => x.Command.Command.CommandId == command.CommandId);
            }
        }

        /// <summary>
        /// Find commands which has been marked as processed but not deleted.
        /// </summary>
        /// <param name="markedAsProcessBefore">Get all commands that were marked as being processed before this date/time.</param>
        /// <returns>Any matching commands or an empty collection.</returns>
        public IEnumerable<DispatchCommand> FindFailedCommands(DateTime markedAsProcessBefore)
        {
            lock (_queue)
            {
                return _queue.Where(x => x.StartedAt < markedAsProcessBefore).Select(x => x.Command).ToList();
            }
        }

        #endregion

        /// <summary>
        /// Get command which was stored first.
        /// </summary>
        /// <returns>Command if any; otherwise <c>null</c>.</returns>
        public DispatchCommand Dequeue()
        {
            MyStruct state;
            lock (_queue)
            {
                state = _queue.FirstOrDefault(x => x.StartedAt == DateTime.MinValue);
                if (state == null)
                    return null;

                state.StartedAt = DateTime.Now;
            }

            return state.Command;
        }

        #region Nested type: MyStruct

        private class MyStruct
        {
            public DispatchCommand Command { get; set; }
            public DateTime StartedAt { get; set; }
        }

        #endregion
    }
}