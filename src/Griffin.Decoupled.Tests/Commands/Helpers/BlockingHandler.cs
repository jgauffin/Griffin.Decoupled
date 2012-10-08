using System;
using System.Threading;
using Griffin.Decoupled.Commands;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    internal class BlockingHandler<T> : IHandleCommand<T> where T : class, ICommand
    {
        private readonly Action<T> _action;
        private readonly ManualResetEventSlim _event = new ManualResetEventSlim(false);

        public BlockingHandler()
        {
        }

        public BlockingHandler(Action<T> action)
        {
            _action = action;
        }

        #region IHandleCommand<T> Members

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

        #endregion

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