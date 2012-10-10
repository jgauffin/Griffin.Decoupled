using System;
using System.Collections.Concurrent;

namespace Griffin.Decoupled.Pipeline
{
    interface IPipelineInvoker
    {
        void InvokeUpstream(UpstreamContext up, object message);
        void InvokeDownstream(DownstreamContext next, object message);
    }
    internal class DownstreamContext : IDownstreamContext
    {
        private readonly ConcurrentQueue<object> _downMessages = new ConcurrentQueue<object>();
        private readonly IDownstreamHandler _mine;
        private readonly ConcurrentQueue<object> _upMessages = new ConcurrentQueue<object>();
        private DownstreamContext _next;
        private UpstreamContext _up;

        public DownstreamContext(IDownstreamHandler mine)
        {
            _mine = mine;
        }

        public bool GotNext
        {
            get { return _next != null; }
        }

        #region IDownstreamContext Members

        public void SendUpstream(object message)
        {
            _up.Invoke(message);
        }

        public void SendDownstream(object message)
        {
            if (_next == null)
                throw new InvalidOperationException("Failed to find a next handler.");
            _next.Invoke(message);
        }

        #endregion

        public void SetNext(DownstreamContext next)
        {
            _next = next;
        }

        public void SetUpstream(UpstreamContext up)
        {
            _up = up;
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