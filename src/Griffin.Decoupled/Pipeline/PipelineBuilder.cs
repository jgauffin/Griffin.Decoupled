using System;
using System.Collections.Generic;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Build the pipeline
    /// </summary>
    public class PipelineBuilder
    {
        private readonly List<DownstreamContext> _downstreamHandlers = new List<DownstreamContext>();
        private readonly List<UpstreamContext> _upstreamHandlers = new List<UpstreamContext>();

        /// <summary>
        /// Register a upstream handler. 
        /// Upstream handlers should be registered from closest to the 
        /// command handler and finally the application handler (which should receive the error messages)
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RegisterUpstream(IUpstreamHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var newContext = new UpstreamContext(handler);
            if (_upstreamHandlers.Count > 0)
                _upstreamHandlers[_upstreamHandlers.Count - 1].SetNext(newContext);
            
            _upstreamHandlers.Add(newContext);
        }

        /// <summary>
        /// Register a downstream handler. Should start with the handler closest to the application and end with the handler which actually dispatches the message to the command handler.
        /// </summary>
        /// <param name="handler">The handler</param>
        public void RegisterDownstream(IDownstreamHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var newContext = new DownstreamContext(handler);
            if (_downstreamHandlers.Count > 0)
                _downstreamHandlers[_downstreamHandlers.Count - 1].SetNext(newContext);

            _downstreamHandlers.Add(newContext);
        }

        /// <summary>
        /// Build the piupeline
        /// </summary>
        public IPipeline Build()
        {
            if (_upstreamHandlers.Count == 0)
                throw new InvalidOperationException("There must be at least one upstream handler (which should take care of any errors).");
            if (_downstreamHandlers.Count == 0)
                throw new InvalidOperationException("There must be at least one downstream handler.");

            var pipeline = new Pipeline();

            foreach (var upstreamHandler in _upstreamHandlers)
            {
                upstreamHandler.SetDownstream(_downstreamHandlers[0]);
                pipeline.Add(upstreamHandler);
            }
            foreach (var downstreamHandler in _downstreamHandlers)
            {
                downstreamHandler.SetUpstream(_upstreamHandlers[0]);
                pipeline.Add(downstreamHandler);
            }

            return pipeline;
        }

    }
}