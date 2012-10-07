using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.DomainEvents.Helpers
{
    class FakeContext : IDownstreamContext
    {
        private readonly Action<object> _down;
        private readonly Action<object> _up;
        public ManualResetEvent _event = new ManualResetEvent(false);

        public FakeContext(Action<object> down = null, Action<object> up = null)
        {
            _down = down;
            _up = up;
        }

        /// <summary>
        /// Send a message back up the chain, typically an error message
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendUpstream(object message)
        {
            Message = message;
            _up(message);
            _event.Set();
        }

        /// <summary>
        /// Send a message towards the command handler
        /// </summary>
        /// <param name="message">Message to forward</param>
        public void SendDownstream(object message)
        {
            Message = message;
            _down(message);
            _event.Set();
        }

        public bool Wait(TimeSpan span)
        {
            return _event.WaitOne(span);
        }

        public object Message { get; private set; }
    }
}
