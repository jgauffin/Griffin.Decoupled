    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Griffin.Decoupled.DomainEvents;
    using Griffin.Decoupled.DomainEvents.Pipeline;
    using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
    using Griffin.Decoupled.Pipeline;
    using Griffin.Decoupled.Tests.DomainEvents.Helpers;
    using NSubstitute;
    using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents.Pipeline
{
    class TransactionalHandlerTests
    {

        [Fact]
        public void UowDispatcherNoUow()
        {
            var uowAdapter = new FakeUowAdapter();
            var storage = new MemoryStorage();
            var context = Substitute.For<IDownstreamContext>();
            var msg = new DispatchEvent(new FakeEvent());

            var handler = new TransactionalHandler(uowAdapter, storage);
            handler.HandleDownstream(context, msg);

            context.Received().SendDownstream(msg);
        }

        [Fact]
        public void UowDispatcher_UowNotReleased()
        {
            var uowAdapter = new FakeUowAdapter();
            var storage = Substitute.For<IDomainEventStorage>();
            var context = Substitute.For<IDownstreamContext>();
            var msg = new DispatchEvent(new FakeEvent());

            var handler = new TransactionalHandler(uowAdapter, storage);
            uowAdapter.Observer.Create(uowAdapter);
            handler.HandleDownstream(context,  msg);
            

            storage.Received().Hold();
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
