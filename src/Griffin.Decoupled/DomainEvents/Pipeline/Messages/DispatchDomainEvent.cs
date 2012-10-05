using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    public class DispatchDomainEvent
    {
        private readonly IDomainEvent _domainEvent;

        public DispatchDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvent = domainEvent;
        }

        public IDomainEvent DomainEvent
        {
            get { return _domainEvent; }
        }
    }
}
