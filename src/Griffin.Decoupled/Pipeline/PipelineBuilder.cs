using System;
using System.Collections.Generic;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Build the pipeline
    /// </summary>
    public class PipelineBuilder
    {
        private readonly List<IDownstreamHandler> _downstreamHandlers = new List<IDownstreamHandler>();
        private readonly List<IUpstreamHandler> _upstreamHandlers = new List<IUpstreamHandler>();

        /// <summary>
        /// Register a upstream handler. 
        /// Upstream handlers should be registered from closest to the 
        /// command handler and finally the application handler (which should receive the error messages)
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void RegisterUpstream(IUpstreamHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            _upstreamHandlers.Add(handler);
        }

        /// <summary>
        /// Register a downstream handler. Should start with the handler closest to the application and end with the handler which actually dispatches the message to the command handler.
        /// </summary>
        /// <param name="handler">The handler</param>
        public void RegisterDownstream(IDownstreamHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            _downstreamHandlers.Add(handler);
        }

        /// <summary>
        /// Build the piupeline
        /// </summary>
        public IPipeline Build()
        {
            var pipeline = new Pipeline();

            var firstDown = new DownstreamContext(_downstreamHandlers[0]);
            var firstUp = new UpstreamContext(_upstreamHandlers[0]);
            firstDown.SetUpstream(firstUp);
            firstUp.SetDownstream(firstDown);
            pipeline.Add(firstDown);
            pipeline.Add(firstUp);

            ConfigureUpstream(pipeline, firstUp, firstDown);
            ConfigureDownstream(pipeline, firstDown, firstUp);

            return pipeline;
        }

        private void ConfigureDownstream(Pipeline pipeline, DownstreamContext firstDown, UpstreamContext firstUp)
        {
            var lastDown = firstDown;
            pipeline.Add(lastDown);
            for (var i = 1; i < _downstreamHandlers.Count; i++)
            {
                var ctx = new DownstreamContext(_downstreamHandlers[i]);
                ctx.SetUpstream(firstUp);
                lastDown.SetNext(ctx);
                pipeline.Add(ctx);
                lastDown = ctx;
            }
        }

        private void ConfigureUpstream(Pipeline pipeline, UpstreamContext firstUp, DownstreamContext firstDown)
        {
            var lastUp = firstUp;
            pipeline.Add(firstDown);
            for (var i = 1; i < _upstreamHandlers.Count; i++)
            {
                var ctx = new UpstreamContext(_upstreamHandlers[i]);
                ctx.SetDownstream(firstDown);
                lastUp.SetNext(ctx);
                pipeline.Add(ctx);
                lastUp = ctx;
            }
        }
    }
}