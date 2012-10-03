using System;
using System.Collections.Concurrent;

namespace Griffin.Decoupled.Commands.Pipeline
{
    internal class DownstreamContext : IDownstreamContext
    {
        private readonly IDownstreamHandler _mine;
        private UpstreamContext _up;
        private DownstreamContext _next;
        ConcurrentQueue<object> _upMessages = new ConcurrentQueue<object>();
        ConcurrentQueue<object> _downMessages = new ConcurrentQueue<object>();

        public DownstreamContext(IDownstreamHandler mine)
        {
            _mine = mine;
        }

        public void SetNext(DownstreamContext next)
        {
            _next = next;
        }

        public void SetUpstream(UpstreamContext up)
        {
            _up = up;
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
            _mine.HandleDownstream(this, message);
            InvokeUpstream();
            InvokeDownstream();
        }

        public void InvokeUpstream()
        {
            object message;
            while (_upMessages.TryDequeue(out message))
            {
                _next.Invoke(message);
            }
        }

        public void InvokeDownstream()
        {
            if (_next == null && _downMessages.Count > 0)
                throw new InvalidOperationException(string.Format("There is no more downstream handlers after '{0}'.",
                                                                  _mine.GetType().FullName));

            object message;
            while (_downMessages.TryDequeue(out message))
            {
                _up.Invoke(message);
            }
        }
    }
}