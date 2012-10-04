using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    class BlockingDispatcher : ICommandDispatcher
    {
        private readonly int _numberOfCommandsToWait;
        private ManualResetEventSlim _event = new ManualResetEventSlim(false);
        private ManualResetEventSlim _dispatcherBlockEvent = new ManualResetEventSlim(true);
        private int _commands;
        private Action<ICommand> _action;
        public BlockingDispatcher(Action<ICommand> action = null)
        {
            _numberOfCommandsToWait = 1;
            _action = action ?? (state => {});
        }

        public BlockingDispatcher(int numberOfCommandsToWait, Action<ICommand> action = null)
        {
            _numberOfCommandsToWait = numberOfCommandsToWait;
            _action = action ?? (state => { });
        }

        public void BlockDispatchInvocation()
        {
            _dispatcherBlockEvent.Reset();
        }

        public void UnblockDispatchInvocation()
        {
            _dispatcherBlockEvent.Set();
        }

        public void Reset()
        {
            _event.Reset();
        }

        public bool Wait(TimeSpan span)
        {
            return _event.Wait(span);
        }

        public ICommand Command { get; set; }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exiting.</remarks>
        public void Close()
        {
            Closed = true;
        }

        /// <summary>
        /// Closed was called.
        /// </summary>
        public bool Closed { get; set; }

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch<T>(T command) where T : class, ICommand
        {
            Command = command;
            ++_commands;

            _action(command);

            if (_numberOfCommandsToWait <= _commands)
                _event.Set();

            _dispatcherBlockEvent.Wait();            
        }
    }
}
