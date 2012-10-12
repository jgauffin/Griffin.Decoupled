using System;
using System.Threading;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    internal class DownContext : IDownstreamContext
    {
        private readonly Action<object> _down;
        private readonly ManualResetEvent _downEvent = new ManualResetEvent(false);
        private readonly Action<object> _up;
        private readonly ManualResetEvent _upEvent = new ManualResetEvent(false);

        public DownContext(Action<object> down, Action<object> up)
        {
            _up = up;
            _down = down;
        }

        public object Message { get; set; }

        #region IDownstreamContext Members

        /// <summary>
        /// Send a message back up the chain, typically an error message
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendUpstream(IUpstreamMessage message)
        {
            Message = message;
            _upEvent.Set();

            if (_up != null)
                _up(message);
        }

        /// <summary>
        /// Send a message towards the command handler
        /// </summary>
        /// <param name="message">Message to forward</param>
        public void SendDownstream(IDownstreamMessage message)
        {
            Message = message;
            _downEvent.Set();

            if (_down != null)
                _down(message);
        }

        #endregion

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