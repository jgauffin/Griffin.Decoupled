using System;
using System.Collections.Concurrent;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Context implementation
    /// </summary>
    public class UpstreamContext : IUpstreamContext
    {
        private readonly ConcurrentQueue<IDownstreamMessage> _downMessages = new ConcurrentQueue<IDownstreamMessage>();
        private readonly IUpstreamHandler _mine;
        private readonly ConcurrentQueue<IUpstreamMessage> _upMessages = new ConcurrentQueue<IUpstreamMessage>();
        private DownstreamContext _down;
        private UpstreamContext _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpstreamContext" /> class.
        /// </summary>
        /// <param name="mine">The handler that this context is for.</param>
        public UpstreamContext(IUpstreamHandler mine)
        {
            _mine = mine;
        }

        #region IUpstreamContext Members

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="message">Message to send, most commonly <see cref="DispatchCommand" />.</param>
        public void SendUpstream(IUpstreamMessage message)
        {
            _upMessages.Enqueue(message);
        }

        /// <summary>
        /// Try to send something down to the command handler again (or to a downstream handler on the way)
        /// </summary>
        /// <param name="message">Message to send</param>
        public void SendDownstream(IDownstreamMessage message)
        {
            _downMessages.Enqueue(message);
        }

        #endregion

        /// <summary>
        /// Sets the next upstream handler that this one will forward messages to
        /// </summary>
        /// <param name="next">The next.</param>
        public void SetNext(UpstreamContext next)
        {
            if (next == null) throw new ArgumentNullException("next");
            _next = next;
        }

        /// <summary>
        /// Sets the downstream handler which this one will send upstream messages to
        /// </summary>
        /// <param name="down">Down.</param>
        /// <exception cref="System.ArgumentNullException">down</exception>
        public void SetDownstream(DownstreamContext down)
        {
            if (down == null) throw new ArgumentNullException("down");
            _down = down;
        }

        /// <summary>
        /// Invokes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Invoke(IUpstreamMessage message)
        {
            _mine.HandleUpstream(this, message);
            InvokeUpstream();
            InvokeDownstream();
        }

        /// <summary>
        /// Invokes all upstream messages.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void InvokeUpstream()
        {
            if (_next == null && _upMessages.Count > 0)
                throw new InvalidOperationException(string.Format("There is no more upstream handlers after '{0}'.",
                                                                  _mine.GetType().FullName));

            IUpstreamMessage message;
            while (_upMessages.TryDequeue(out message))
            {
                _next.Invoke(message);
            }
        }

        /// <summary>
        /// Invokes all downstream messages.
        /// </summary>
        public void InvokeDownstream()
        {
            IDownstreamMessage message;
            while (_downMessages.TryDequeue(out message))
            {
                _down.Invoke(message);
            }
        }
    }
}