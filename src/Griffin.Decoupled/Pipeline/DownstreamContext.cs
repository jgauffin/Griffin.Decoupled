using System;
using System.Collections.Concurrent;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Used to keep track of a handler (and which it's sibling is)
    /// </summary>
    public class DownstreamContext : IDownstreamContext
    {
        private readonly ConcurrentQueue<IDownstreamMessage> _downMessages = new ConcurrentQueue<IDownstreamMessage>();
        private readonly IDownstreamHandler _mine;
        private readonly ConcurrentQueue<IUpstreamMessage> _upMessages = new ConcurrentQueue<IUpstreamMessage>();
        private DownstreamContext _next;
        private UpstreamContext _up;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownstreamContext" /> class.
        /// </summary>
        /// <param name="mine">The mine.</param>
        public DownstreamContext(IDownstreamHandler mine)
        {
            if (mine == null) throw new ArgumentNullException("mine");
            _mine = mine;
        }

        /// <summary>
        /// Is this the last handler in the pipeline?
        /// </summary>
        public bool IsLast
        {
            get { return _next == null; }
        }

        #region IDownstreamContext Members

        /// <summary>
        /// Sends the upstream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public void SendUpstream(IUpstreamMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (_up == null)
                throw new InvalidOperationException("No upstream handler has been set.");

            _up.Invoke(message);
        }

        /// <summary>
        /// Sends the downstream.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.InvalidOperationException">Failed to find a next handler.</exception>
        public void SendDownstream(IDownstreamMessage message)
        {
            if (_next == null)
                throw new InvalidOperationException("Failed to find a next handler.");
            _next.Invoke(message);
        }

        #endregion

        /// <summary>
        /// Sets the next handler (which this will forward messages to)
        /// </summary>
        /// <param name="next">The next.</param>
        public void SetNext(DownstreamContext next)
        {
            if (next == null) throw new ArgumentNullException("next");
            _next = next;
        }

        /// <summary>
        /// The the upstream handler that this handler will forward messages to
        /// </summary>
        /// <param name="up">upstream handler</param>
        public void SetUpstream(UpstreamContext up)
        {
            if (up == null) throw new ArgumentNullException("up");
            _up = up;
        }

        /// <summary>
        /// Invokes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Invoke(IDownstreamMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            _mine.HandleDownstream(this, message);
            InvokeUpstream();
            InvokeDownstream();
        }

        /// <summary>
        /// Invokes all upstream messages.
        /// </summary>
        public void InvokeUpstream()
        {
            IUpstreamMessage message;
            while (_upMessages.TryDequeue(out message))
            {
                _up.Invoke(message);
            }
        }

        /// <summary>
        /// Invokes all downstream messages
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void InvokeDownstream()
        {
            if (_next == null && _downMessages.Count > 0)
                throw new InvalidOperationException(string.Format("There is no more downstream handlers after '{0}'.",
                                                                  _mine.GetType().FullName));

            IDownstreamMessage message;
            while (_downMessages.TryDequeue(out message))
            {
                _next.Invoke(message);
            }
        }
    }
}