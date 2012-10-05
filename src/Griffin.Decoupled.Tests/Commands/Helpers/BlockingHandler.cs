using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    class BlockingHandler<T> : IHandleCommand<T> where T : class, ICommand
    {
        ManualResetEventSlim _event = new ManualResetEventSlim(false);
        private Action<T> _action;

        public BlockingHandler() { }
        public BlockingHandler(Action<T> action)
        {
            _action = action;
        }

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="command">Command to run</param>
        public void Invoke(T command)
        {
            if (_action != null)
                _action(command);

            _event.Set();
        }

        public void Reset()
        {
            _event.Reset();
        }

        public bool Wait(TimeSpan span)
        {
            return _event.Wait(span);
        }
    }
}
