using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    class DownContext : IDownstreamContext
    {
        private readonly Action<object> _up;
        private readonly Action<object> _down;
        ManualResetEvent _downEvent = new ManualResetEvent(false);
        ManualResetEvent _upEvent = new ManualResetEvent(false);

        public DownContext(Action<object> down, Action<object> up)
        {
            _up = up;
            _down = down;
        }
        
        /// <summary>
        /// Send a message back up the chain, typically an error message
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendUpstream(object message)
        {
            _upEvent.Set();

            if (_up != null)
                _up(message);
        }

        /// <summary>
        /// Send a message towards the command handler
        /// </summary>
        /// <param name="message">Message to forward</param>
        public void SendDownstream(object message)
        {
            _downEvent.Set();

            if (_down != null)
                _down(message);
        }

        public bool WaitDown(TimeSpan span)
        {
            return _downEvent.WaitOne(span);
        }

        public bool WaitUp(TimeSpan span)
        {
            return _upEvent.WaitOne(span);
        }

        public void ResetDown()
        {
            _downEvent.Reset();
        }
    }
}
