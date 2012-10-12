using System;
using System.Collections.Generic;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline.Messages;

namespace Griffin.Decoupled.Pipeline
{
    /// <summary>
    /// Our own implementation of a pipeline
    /// </summary>
    /// <remarks>The last handler isn't included in the downstream pipeline, it's purpose is just to receive everything
    /// that goes wrong.</remarks>
    public class Pipeline : IPipeline, IUpstreamHandler, IDownstreamHandler
    {
        private readonly List<DownstreamContext> _downstream = new List<DownstreamContext>();
        private readonly DownstreamContext _myDownContext;
        private readonly UpstreamContext _myUpContext;
        private readonly List<UpstreamContext> _upstream = new List<UpstreamContext>();
        private bool _fixed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline" /> class.
        /// </summary>
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
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        void IDownstreamHandler.HandleDownstream(IDownstreamContext context, IDownstreamMessage message)
        {
            if (message is DispatchCommand)
                _upstream[0].Invoke(new CommandCompleted((DispatchCommand) message));
            else if (message is DispatchEvent)
                _upstream[0].Invoke(new EventCompleted((DispatchEvent) message));

            // just ignore everything else.
        }

        #endregion

        #region IPipeline Members

        /// <summary>
        /// Send something down the pipeline (usually the invoke command message)
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public void Send(IDownstreamMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            if (!_fixed)
                throw new InvalidOperationException("Start() must be called first.");
            _downstream[0].Invoke(message);
        }

        /// <summary>
        /// Set upstream destination
        /// </summary>
        /// <param name="handler">Will receive all upstream messages</param>
        /// <exception cref="System.ArgumentNullException">handler</exception>
        /// <remarks>Must be the last configured action on the pipeline</remarks>
        public void SetDestination(IUpstreamHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (_downstream.Count == 0)
                throw new InvalidOperationException("You must have set at least one downstream handler before invoking SetDestination().");

            var ctx = new UpstreamContext(handler);
            ctx.SetNext(_myUpContext);
            ctx.SetDownstream(_downstream[0]);
            _upstream.Add(ctx);
        }

        /// <summary>
        /// MUST be invoked before the pipeline can be used.
        /// </summary>
        public void Start()
        {
            foreach (var upstreamContext in _upstream)
            {
                upstreamContext.SetDownstream(_downstream[0]);
            }
            foreach (var downstreamContext in _downstream)
            {
                downstreamContext.SetUpstream(_upstream[0]);
            }
            _fixed = true;
            Send(new StartHandlers());
        }

        #endregion

        #region IUpstreamHandler Members

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        void IUpstreamHandler.HandleUpstream(IUpstreamContext context, IUpstreamMessage message)
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
                    "The pipeline may not be modified after Start() has been called.");

            if (context.IsLast)
                context.SetNext(_myDownContext);
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
                    "The pipeline may not be modified after Sart() has been called.");

            _upstream.Add(context);
        }
    }
}