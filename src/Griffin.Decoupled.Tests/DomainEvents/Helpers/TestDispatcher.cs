using System;
using System.Collections.Generic;
using System.Threading;
using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class TestDispatcher : IDomainEventDispatcher
    {
        private readonly bool _throwException;
        private ManualResetEvent _theEvent = new ManualResetEvent(false);
        private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public TestDispatcher()
        {
            
        }
        public TestDispatcher(bool throwException)
        {
            _throwException = throwException;
        }


        public IEnumerable<IDomainEvent> DomainEvents
        {
            get { return _domainEvents; }
        }

        public void Wait(TimeSpan timeSpan)
        {
            if (!_theEvent.WaitOne(timeSpan))
                throw new InvalidOperationException("Timeout expired.");
        }

        /// <summary>
        /// Dispatch domain event.
        /// </summary>
        /// <typeparam name="T">Domain event type</typeparam>
        /// <param name="domainEvent">The domain event</param>
        public void Dispatch<T>(T domainEvent) where T : class, IDomainEvent
        {
            _domainEvents.Add(domainEvent);
            _theEvent.Set();

            if (_throwException)
                throw new Exception("The exception");
        }
    }
}