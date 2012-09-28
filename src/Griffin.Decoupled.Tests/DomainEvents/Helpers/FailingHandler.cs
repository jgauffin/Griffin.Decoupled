using System;
using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.Tests.DomainEvents.Helpers
{
    public class FailingHandler<T> : ISubscribeOn<T> where T : class, IDomainEvent
    {
        #region ISubscribeOn<T> Members

        /// <summary>
        /// Will be invoked when the domain event is triggered.
        /// </summary>
        /// <param name="domainEvent">Domin event to handle</param>
        public void Handle(T domainEvent)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}