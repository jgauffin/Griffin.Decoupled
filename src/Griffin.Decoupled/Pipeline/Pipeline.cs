using System;
using System.Collections.Generic;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Our own implementation of a pipeline
    /// </summary>
    /// <remarks>The last handler isn't included in the downstream pipeline, it's purpose is just to receive everything
    /// that goes wrong.</remarks>
    internal class Pipeline : IPipeline, IUpstreamHandler, IDownstreamHandler
    {
        private readonly List<DownstreamContext> _downstream = new List<DownstreamContext>();
        private readonly DownstreamContext _myDownContext;
        private readonly UpstreamContext _myUpContext;
        private readonly List<UpstreamContext> _upstream = new List<UpstreamContext>();
        private bool _fixed;

        public Pipeline()
        {
            _myUpContext = new UpstreamContext(this);
            _myDownContext = new DownstreamContext(this);
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="SendCommand"/>.</param>
        void IDownstreamHandler.HandleDownstream(IDownstreamContext context, object message)
        {
            throw new InvalidOperationException(_downstream[_downstream.Count - 2] +
                                                " invoked SendDownstream when its the last handler. Don't do that..");
        }

        #endregion

        #region IPipeline Members

        /// <summary>
        /// Send something down the pipeline (usually the invoke command message)
        /// </summary>
        /// <param name="message">Message to send</param>
        public void Send(object message)
        {
            _downstream[0].Invoke(message);
        }

        /// <summary>
        /// Set upstream destination
        /// </summary>
        /// <param name="handler">Will receive all upstream messages</param>
        public void SetDestination(IUpstreamHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            var ctx = new UpstreamContext(handler);
            ctx.SetNext(_myUpContext);
            ctx.SetDownstream(_downstream[0]);
            _upstream.Add(ctx);

            _downstream[_downstream.Count - 1].SetNext(_myDownContext);

            _fixed = true;
        }

        #endregion

        #region IUpstreamHandler Members

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        void IUpstreamHandler.HandleUpstream(IUpstreamContext context, object message)
        {
            throw new InvalidOperationException(_upstream[_upstream.Count - 2] +
                                                " invoked SendUpstream when its the last handler. Don't do that..");
        }

        #endregion

        /// <summary>
        /// Add a new downstream handler (from the invoker down to the command handler)
        /// </summary>
        /// <param name="context">Context for the handler</param>
        public void Add(DownstreamContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (_fixed)
                throw new InvalidOperationException(
                    "The pipeline may not be modified after SetDestination has been called.");
            _downstream.Add(context);
        }

        /// <summary>
        /// Add a new upstream handler (handles failures which was detected when the command was traveling down to the command handler)
        /// </summary>
        /// <param name="context">Context for the handler</param>
        public void Add(UpstreamContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (_fixed)
                throw new InvalidOperationException(
                    "The pipeline may not be modified after SetDestination has been called.");
            _upstream.Add(context);
        }
    }
}