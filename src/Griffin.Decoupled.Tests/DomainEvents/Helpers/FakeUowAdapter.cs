using Griffin.Decoupled.DomainEvents;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class FakeUowAdapter : IUnitOfWorkAdapter
    {
        public IUnitOfWorkObserver Observer { get; private set; }

        #region IUnitOfWorkAdapter Members

        /// <summary>
        /// Register our own observer which is used to control when the domain events are dispatched.
        /// </summary>
        /// <param name="observer">Our observer.</param>
        public void Register(IUnitOfWorkObserver observer)
        {
            Observer = observer;
        }

        #endregion
    }
}