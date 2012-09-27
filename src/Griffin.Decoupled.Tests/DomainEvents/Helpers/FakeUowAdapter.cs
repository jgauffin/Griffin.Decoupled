using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class FakeUowAdapter : IUnitOfWorkAdapter
    {
        /// <summary>
        /// Register our own observer which is used to control when the domain events are dispatched.
        /// </summary>
        /// <param name="observer">Our observer.</param>
        public void Register(IUnitOfWorkObserver observer)
        {
            Observer = observer;
        }

        public IUnitOfWorkObserver Observer { get; private set; }
    }
}