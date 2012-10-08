namespace Griffin.Decoupled.DomainEvents.Pipeline.Messages
{
    public class DispatchEvent
    {
        private readonly IDomainEvent _domainEvent;

        public DispatchEvent(IDomainEvent domainEvent)
        {
            _domainEvent = domainEvent;
        }

        public IDomainEvent DomainEvent
        {
            get { return _domainEvent; }
        }
    }
}