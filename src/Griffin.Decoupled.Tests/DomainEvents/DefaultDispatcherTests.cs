using System;
using System.Linq;
using System.Threading;
using Griffin.Decoupled.DomainEvents;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class DefaultDispatcherTests
    {
        [Fact]
        public void RegularDispatch()
        {
            var innerDispatcher = new TestDispatcher();
            var domainEvent = Substitute.For<IDomainEvent>();
            var dispatcher = new DefaultDispatcher(innerDispatcher);

            dispatcher.Dispatch(domainEvent);
            innerDispatcher.Wait(TimeSpan.FromSeconds(1));

            Assert.Same(domainEvent, innerDispatcher.DomainEvents.First());
        }

        [Fact]
        public void HandlerCrashes()
        {
            var innerDispatcher = new TestDispatcher(true);
            var domainEvent = Substitute.For<IDomainEvent>();
            var dispatcher = new DefaultDispatcher(innerDispatcher);
            var triggerEvent = new ManualResetEvent(false);
            dispatcher.DispatcherFailed += (sender, args) => triggerEvent.Set();

            dispatcher.Dispatch(domainEvent);
            innerDispatcher.Wait(TimeSpan.FromSeconds(1));

            Assert.True(triggerEvent.WaitOne(1000));
        }

        [Fact]
        public void UowDispatcherNoUow()
        {
            var uowAdapter = new FakeUowAdapter();
            var innerDispatcher = new TestDispatcher(true);
            var domainEvent = Substitute.For<IDomainEvent>();
            var dispatcher = new DefaultDispatcher(innerDispatcher, uowAdapter);
            var triggerEvent = new ManualResetEvent(false);
            dispatcher.DispatcherFailed += (sender, args) => triggerEvent.Set();

            dispatcher.Dispatch(domainEvent);
            innerDispatcher.Wait(TimeSpan.FromSeconds(1));

            Assert.True(triggerEvent.WaitOne(1000));
        }

        [Fact]
        public void UowDispatcher_UowNotReleased()
        {
            var uowAdapter = new FakeUowAdapter();
            var innerDispatcher = new TestDispatcher(true);
            var domainEvent = Substitute.For<IDomainEvent>();
            var dispatcher = new DefaultDispatcher(innerDispatcher, uowAdapter);
            var triggerEvent = new ManualResetEvent(false);
            dispatcher.DispatcherFailed += (sender, args) => triggerEvent.Set();

            uowAdapter.Observer.Create(uowAdapter);
            dispatcher.Dispatch(domainEvent);

            Assert.False(triggerEvent.WaitOne(1000));
        }

        [Fact]
        public void UowDispatcher_UowReleasedSuccessfully()
        {
            var uowAdapter = new FakeUowAdapter();
            var innerDispatcher = new TestDispatcher(true);
            var domainEvent = Substitute.For<IDomainEvent>();
            var dispatcher = new DefaultDispatcher(innerDispatcher, uowAdapter);
            var triggerEvent = new ManualResetEvent(false);
            dispatcher.DispatcherFailed += (sender, args) => triggerEvent.Set();

            uowAdapter.Observer.Create(uowAdapter);
            dispatcher.Dispatch(domainEvent);
            uowAdapter.Observer.Released(uowAdapter, true);

            Assert.True(triggerEvent.WaitOne(1000));
        }


        [Fact]
        public void UowDispatcher_UowReleasedFailed()
        {
            var uowAdapter = new FakeUowAdapter();
            var innerDispatcher = new TestDispatcher(true);
            var domainEvent = Substitute.For<IDomainEvent>();
            var dispatcher = new DefaultDispatcher(innerDispatcher, uowAdapter);
            var triggerEvent = new ManualResetEvent(false);
            dispatcher.DispatcherFailed += (sender, args) => triggerEvent.Set();

            uowAdapter.Observer.Create(uowAdapter);
            dispatcher.Dispatch(domainEvent);
            uowAdapter.Observer.Released(uowAdapter, false);

            Assert.False(triggerEvent.WaitOne(1000));
        }
    }
}