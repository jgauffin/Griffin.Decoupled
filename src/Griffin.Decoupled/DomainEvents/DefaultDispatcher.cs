using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Griffin.Decoupled.DomainEvents
{
    /// <summary>
    /// The default domain event dispatcher
    /// </summary>
    /// <remarks>
    /// <para>This dispatcher can wait on transactions to complete (you can find out why by reading <see cref="IDomainEventStorage"/>). It will also
    /// trigger the events asynchronously (each event will however be executed async when it comes to all subscribers for it).</para>
    /// <para>
    /// Let the inner dispatcher throw exceptions while dispatching. We'll pick the exceptions up and dispatch them along to a handler.
    /// </para>
    /// <para>This class is designed to be a singleton which is used during the entire application lifetime.</para>
    /// </remarks>
    public class DefaultDispatcher : IDomainEventDispatcher, IUnitOfWorkObserver
    {
        private const int MaxWorkers = 5;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ConcurrentQueue<IDomainEvent> _queue = new ConcurrentQueue<IDomainEvent>();
        private readonly IDomainEventStorage _storage;
        private readonly ThreadedUowMapper _threadedUowMapper = new ThreadedUowMapper();
        private long _currentWorkers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDispatcher" /> class.
        /// </summary>
        /// <param name="dispatcher">The real dispatcher to use, for instance <see cref="ContainerDispatcher"/>.</param>
        public DefaultDispatcher(IDomainEventDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");

            _dispatcher = dispatcher;
            _storage = new MemoryStorage();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDispatcher" /> class.
        /// </summary>
        /// <param name="dispatcher">The real dispatcher to use, for instance <see cref="ContainerDispatcher"/>.</param>
        /// <param name="unitOfWorkAdapter">The unit of work adapter (to dispatch events once transactions complete) See the class doc for more information.</param>
        public DefaultDispatcher(IDomainEventDispatcher dispatcher, IUnitOfWorkAdapter unitOfWorkAdapter)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            if (unitOfWorkAdapter == null) throw new ArgumentNullException("unitOfWorkAdapter");

            _dispatcher = dispatcher;
            _storage = new MemoryStorage();
            unitOfWorkAdapter.Register(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDispatcher" /> class.
        /// </summary>
        /// <param name="dispatcher">The real dispatcher to use, for instance <see cref="ContainerDispatcher" />.</param>
        /// <param name="unitOfWorkAdapter">The unit of work adapter (to dispatch events once transactions complete) See the class doc for more information.</param>
        /// <param name="storage">Where to store events while we wait on execution.</param>
        public DefaultDispatcher(IDomainEventDispatcher dispatcher, IUnitOfWorkAdapter unitOfWorkAdapter,
                                 IDomainEventStorage storage)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            if (unitOfWorkAdapter == null) throw new ArgumentNullException("unitOfWorkAdapter");
            if (storage == null) throw new ArgumentNullException("storage");

            _dispatcher = dispatcher;
            _storage = storage;
            unitOfWorkAdapter.Register(this);
        }

        #region IDomainEventDispatcher Members

        /// <summary>
        /// Dispatch domain event.
        /// </summary>
        /// <typeparam name="T">Domain event type</typeparam>
        /// <param name="domainEvent">The domain event</param>
        public void Dispatch<T>(T domainEvent) where T : class, IDomainEvent
        {
            _queue.Enqueue(domainEvent);

            var batchId = _threadedUowMapper.GetBatchId();
            if (batchId != Guid.Empty)
            {
                _storage.Store(batchId, domainEvent);
                return;
            }

            // Not very much thread safe.
            if (Interlocked.Read(ref _currentWorkers) < MaxWorkers)
            {
                Interlocked.Increment(ref _currentWorkers);
                ThreadPool.QueueUserWorkItem(DispatchEventNow);
            }
        }

        #endregion

        #region IUnitOfWorkObserver Members

        /// <summary>
        /// A UoW has been created for the current thread.
        /// </summary>
        void IUnitOfWorkObserver.Create(object unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _threadedUowMapper.Create(unitOfWork);
        }

        /// <summary>
        /// A UoW has been released for the current thread.
        /// </summary>
        /// <param name="unitOfWork">Unit of work which was created previously by Create()</param>
        /// <param name="successful"><c>true</c> if the UoW was saved successfully; otherwise <c>false</c>.</param>
        void IUnitOfWorkObserver.Released(object unitOfWork, bool successful)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");

            var batchId = _threadedUowMapper.Release(unitOfWork);
            if (!successful)
            {
                _storage.Delete(batchId);
                return;
            }

            var domainEvents = _storage.Load(batchId);
            foreach (var domainEvent in domainEvents)
            {
                ExecuteEvent(domainEvent);
            }
            _storage.Delete(batchId);
        }

        #endregion

        private void DispatchEventNow(object state)
        {
            try
            {
                IDomainEvent theEvent;
                if (_queue.TryDequeue(out theEvent))
                    ExecuteEvent(theEvent);
            }
            finally
            {
                Interlocked.Decrement(ref _currentWorkers);
            }
        }


        private void ExecuteEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null) throw new ArgumentNullException("domainEvent");

            try
            {
                _dispatcher.Dispatch(domainEvent);
            }
            catch (Exception err)
            {
                DispatcherFailed(this, new DispatcherFailedEventArgs(err, domainEvent));
            }
        }

        /// <summary>
        /// Invoked when the inner dispatcher failed to deliver the event properly.
        /// </summary>
        /// <remarks>This usually means that one of the <c><![CDATA[ISubscribeOn<T>]]></c> handlers have thrown an exception.</remarks>
        public event EventHandler<DispatcherFailedEventArgs> DispatcherFailed = delegate { };
    }
}