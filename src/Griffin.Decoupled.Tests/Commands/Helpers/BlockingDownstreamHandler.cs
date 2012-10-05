using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.Tests.Commands.Helpers
{
    public delegate void DownHandler(IDownstreamContext context, object message);

    class BlockingDownstreamHandler : IDownstreamHandler
    {
        private readonly DownHandler _handler;
        ManualResetEvent _event = new ManualResetEvent(false);
        
        public BlockingDownstreamHandler(DownHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="SendCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            _event.Set();

            _handler(context, message);
        }

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
