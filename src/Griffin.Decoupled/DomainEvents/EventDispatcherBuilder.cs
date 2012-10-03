using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.DomainEvents
{
    public class EventDispatcherBuilder
    {
        private IDomainEventStorage _storage = new MemoryStorage();
        private IUnitOfWorkAdapter _unitOfWorkAdapter;
        private IDomainEventDispatcher _inner;

        public EventDispatcherBuilder StoreEvents(IDomainEventStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
            return this;
        }

        public EventDispatcherBuilder UseContainer(IRootContainer rootContainer)
        {
            if (rootContainer == null) throw new ArgumentNullException("rootContainer");
            _inner = new ContainerDispatcher(rootContainer);
            return this;
        }

        public EventDispatcherBuilder UseCustomDispatcher(IDomainEventDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _inner = dispatcher;
            return this;
        }

        public EventDispatcherBuilder Transactional(IUnitOfWorkAdapter adapter)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            _unitOfWorkAdapter = adapter;
            return this;
        }

        public IDomainEventDispatcher Build()
        {
            if (_inner == null)
                throw new InvalidOperationException("You must specify a dispatcher, either through the UseContainer() method or by the UseCustomDispatcher() method.");

            var dispatcher = _unitOfWorkAdapter == null
                                 ? new DefaultDispatcher(_inner, _storage)
                                 : new DefaultDispatcher(_inner, _unitOfWorkAdapter, _storage);

            return dispatcher;
        }

    }
}
