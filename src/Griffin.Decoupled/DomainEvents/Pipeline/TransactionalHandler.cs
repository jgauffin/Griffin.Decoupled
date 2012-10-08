using System;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;

namespace Griffin.Decoupled.DomainEvents.Pipeline
{
    /// <summary>
    /// Holds messages until their transaction has been committed.
    /// </summary>
    /// <remarks>The intended purpose of this handler is to make sure that the domain objects (or just your business layer) have successfully
    /// committed everything into the data source before the domain events are distributed. Transaction failure will just make the handler
    /// delete all domain events (which was registered when the transaction was active) without dispatching them.</remarks>
    public class TransactionalHandler : IDownstreamHandler, IUnitOfWorkObserver
    {
        private readonly IDomainEventStorage _storage;
        private readonly IThreadBatchIdMapper _threadBatchIdMapper = new ThreadBatchIdMapper();
        private IDownstreamContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalHandler" /> class.
        /// </summary>
        /// <param name="adapter">Used to monitor all transactions.</param>
        /// <param name="storage">Used to temporarily store domain events until the transaction have been committed.</param>
        public TransactionalHandler(IUnitOfWorkAdapter adapter, IDomainEventStorage storage)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
            adapter.Register(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalHandler" /> class.
        /// </summary>
        /// <param name="adapter">Used to monitor all transactions.</param>
        /// <param name="storage">Used to temporarily store domain events until the transaction have been committed.</param>
        /// <param name="threadBatchIdMapper">Used to map batch ids to threads</param>
        public TransactionalHandler(IUnitOfWorkAdapter adapter, IDomainEventStorage storage,
                                    IThreadBatchIdMapper threadBatchIdMapper)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
            adapter.Register(this);
            _threadBatchIdMapper = threadBatchIdMapper;
        }

        #region IDownstreamHandler Members

        /// <summary>
        /// Send a message to the command handler
        /// </summary>
        /// <param name="context">my context</param>
        /// <param name="message">Message to send, typically <see cref="DispatchCommand"/>.</param>
        public void HandleDownstream(IDownstreamContext context, object message)
        {
            _context = context;

            var msg = message as DispatchEvent;
            if (msg != null)
            {
                var batchId = _threadBatchIdMapper.GetBatchId();
                if (batchId != Guid.Empty)
                {
                    _storage.Hold(batchId, msg.DomainEvent);
                    return;
                }
            }


            context.SendDownstream(message);
        }

        #endregion

        #region IUnitOfWorkObserver Members

        /// <summary>
        /// A UoW has been created for the current thread.
        /// </summary>
        /// <param name="unitOfWork">The unit of work that was created.</param>
        public void Create(object unitOfWork)
        {
            _threadBatchIdMapper.Create(unitOfWork);
        }

        /// <summary>
        /// A UoW has been released for the current thread.
        /// </summary>
        /// <param name="unitOfWork">UoW which was released. Must be same as in <see cref="IUnitOfWorkObserver.Create"/>.</param>
        /// <param name="successful"><c>true</c> if the UoW was saved successfully; otherwise <c>false</c>.</param>
        public void Released(object unitOfWork, bool successful)
        {
            var guid = _threadBatchIdMapper.Release(unitOfWork);
            if (successful)
            {
                var jobs = _storage.Release(guid);
                foreach (var job in jobs)
                {
                    _context.SendDownstream(new DispatchEvent(job));
                }
            }
            else
            {
                _storage.Delete(guid);
            }
        }

        #endregion
    }
}