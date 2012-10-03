using System;
using System.Collections.Concurrent;

namespace Griffin.Decoupled.Commands.Pipeline
{
    /// <summary>
    /// Context implementation
    /// </summary>
    internal class UpstreamContext : IUpstreamContext
    {
        private readonly IUpstreamHandler _mine;
        private UpstreamContext _next;
        private DownstreamContext _down;
        ConcurrentQueue<object> _upMessages = new ConcurrentQueue<object>();
        ConcurrentQueue<object> _downMessages = new ConcurrentQueue<object>();

        public UpstreamContext(IUpstreamHandler mine)
        {
            _mine = mine;
        }

        public void SetNext(UpstreamContext next)
        {
            _next = next;
        }

        public void SetDownstream(DownstreamContext down)
        {
            _down = down;
        }

        public void SendUpstream(object message)
        {
            _upMessages.Enqueue(message);
        }

        public void SendDownstream(object message)
        {
            _downMessages.Enqueue(message);
        }

        public void Invoke(object message)
        {
            _mine.HandleUpstream(this, message);
            InvokeUpstream();
            InvokeDownstream();
        }

        public void InvokeUpstream()
        {
            if (_next == null && _upMessages.Count > 0)
                throw new InvalidOperationException(string.Format("There is no more upstream handlers after '{0}'.",
                                                                  _mine.GetType().FullName));

            object message;
            while (_upMessages.TryDequeue(out message))
            {
                _next.Invoke(message);
            }
        }

        public void InvokeDownstream()
        {
            object message;
            while (_downMessages.TryDequeue(out message))
            {
                _down.Invoke(message);
            }
        }
    }
}