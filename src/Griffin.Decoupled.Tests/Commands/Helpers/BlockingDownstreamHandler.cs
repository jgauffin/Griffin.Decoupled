using System;
using System.Threading;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    public delegate void DownHandler(IDownstreamContext context, object message);

    internal class BlockingDownstreamHandler : IDownstreamHandler
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(false);
        private readonly DownHandler _handler;

        public BlockingDownstreamHandler(DownHandler handler)
        {
            _handler = handler;
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            _event.Set();

            _handler(context, message);
        }

        #endregion

        public bool Wait(TimeSpan span)
        {
            return _event.WaitOne(span);
        }

        public void Reset()
        {
            _event.Reset();
        }
    }
}