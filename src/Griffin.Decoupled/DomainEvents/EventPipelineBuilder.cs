using System;
using Griffin.Decoupled.DomainEvents.Pipeline;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// Constructs a pipeline dispatcher.
    /// </summary>
    public class EventPipelineBuilder
    {
        private readonly IUpstreamHandler _errorHandler;
        private IDownstreamHandler _inner;
        private int _maxWorkers;
        private IDomainEventStorage _storage = new MemoryStorage();
        private IUnitOfWorkAdapter _unitOfWorkAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPipelineBuilder" /> class.
        /// </summary>
        /// <param name="errorHandler">Your upstream handle which will receive and take care of all error messages (which are defined in <c>Griffin.Decoupled.DomainEvents.Pipeline.Messages</c>).</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public EventPipelineBuilder(IUpstreamHandler errorHandler)
        {
            if (errorHandler == null) throw new ArgumentNullException("errorHandler");
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// Use asynchrounous dispatching
        /// </summary>
        /// <param name="maxWorkers">The maximum number of events which can be dispatched simultaneously.</param>
        /// <returns></returns>
        public EventPipelineBuilder Asynchronous(int maxWorkers = 5)
        {
            _maxWorkers = maxWorkers;
            return this;
        }

        /// <summary>
        /// Custom strage for events which awaits transaction completition.
        /// </summary>
        /// <param name="storage">Event storage</param>
        /// <returns>this</returns>
        public EventPipelineBuilder StoreEvents(IDomainEventStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
            return this;
        }

        /// <summary>
        /// Use an inversion of control container to find the dispatchers.
        /// </summary>
        /// <param name="rootContainer">Container adapter.</param>
        /// <returns>this</returns>
        public EventPipelineBuilder UseContainer(IRootContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");
            _inner = new IocHandler(rootContainer);
            return this;
        }

        /// <summary>
        /// Use a custom dispatcher for identifying the subscribers.
        /// </summary>
        /// <param name="dispatcher">Custom dispatcher. </param>
        /// <returns>this</returns>
        /// <remarks>The events are sent in the <see cref="DispatchEvent"/> message, so just check the message variable in <see cref="IDownstreamHandler.HandleDownstream"/> method after that.</remarks>
        public EventPipelineBuilder UseCustomDispatcher(IDownstreamHandler dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _inner = dispatcher;
            return this;
        }

        /// <summary>
        /// Wait until transactions have been completed before events are dispatched.
        /// </summary>
        /// <param name="adapter">Adapter used to detect when transactions have been committed/rolled back.</param>
        /// <returns>this</returns>
        /// <seealso cref="TransactionalHandler"/>
        public EventPipelineBuilder WaitOnTransactions(IUnitOfWorkAdapter adapter)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            _unitOfWorkAdapter = adapter;
            return this;
        }

        public IDomainEventDispatcher Build()
        {
            if (_inner == null)
                throw new InvalidOperationException(
                    "You must specify a dispatcher, either through the UseContainer() method or by the UseCustomDispatcher() method.");

            var builder = new PipelineBuilder();
            if (_unitOfWorkAdapter != null)
                builder.RegisterDownstream(new TransactionalHandler(_unitOfWorkAdapter, _storage));

            if (_maxWorkers > 0)
                builder.RegisterDownstream(new AsyncHandler(_maxWorkers));

            builder.RegisterDownstream(_inner);

            builder.RegisterUpstream(_errorHandler);

            return new EventPipelineDispatcher(builder.Build());
        }
    }
}