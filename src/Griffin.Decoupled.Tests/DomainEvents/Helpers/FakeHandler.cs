using System;
using System.Threading;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.DomainEvents.Helpers
{
    internal class FakeContext : IDownstreamContext
    {
        private readonly Action<object> _down;
        private readonly Action<object> _up;
        public ManualResetEvent _event = new ManualResetEvent(false);

        public FakeContext(Action<object> down = null, Action<object> up = null)
        {
            _down = down;
            _up = up;
        }

        public object Message { get; private set; }

        #region IDownstreamContext Members

        /// <summary>
        /// Send a message back up the chain, typically an error message
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendUpstream(object message)
        {
            Message = message;
            _event.Set();

            if (_up != null)
                _up(message);
        }

        /// <summary>
        /// Send a message towards the command handler
        /// </summary>
        /// <param name="message">Message to forward</param>
        public void SendDownstream(object message)
        {
            Message = message;
            _event.Set();
            if (_down != null)
                _down(message);
        }

        #endregion

        public bool Wait(TimeSpan span)
        {
            return _event.WaitOne(span);
        }

        public void Reset()
        {
            _event.Reset();
            Message = null;
        }
    }
}