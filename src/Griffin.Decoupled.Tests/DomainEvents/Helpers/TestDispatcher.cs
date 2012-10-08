using System;
using System.Collections.Generic;
using System.Threading;
using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class TestDispatcher : IDomainEventDispatcher
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        private readonly ManualResetEvent _theEvent = new ManualResetEvent(false);
        private readonly bool _throwException;

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

        #region IDomainEventDispatcher Members

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

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all events are propagated before returning.</remarks>
        public void Close()
        {
        }

        #endregion

        public void Wait(TimeSpan timeSpan)
        {
            if (!_theEvent.WaitOne(timeSpan))
                throw new InvalidOperationException("Timeout expired.");
        }
    }
}