using System.Collections.Generic;

namespace Griffin.Decoupled.Commands.Pipeline
{
    internal class Pipeline : IPipeline
    {
        List<UpstreamContext> _upstream = new List<UpstreamContext>();
        List<DownstreamContext> _downstream = new List<DownstreamContext>();

        public void Send(object message)
        {
            _downstream[0].Invoke(message);
        }

        public void Add(DownstreamContext context)
        {
            _downstream.Add(context);
        }

        public void Add(UpstreamContext context)
        {
            _upstream.Add(context);
        }
    }
}