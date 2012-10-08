using Griffin.Decoupled.DomainEvents;
using Griffin.Decoupled.DomainEvents.Pipeline;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents.Pipeline
{
    public class TransactionalHandlerTests
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
            var uowMapper = new ThreadBatchIdMapper();

            var handler = new TransactionalHandler(uowAdapter, storage, uowMapper);
            uowAdapter.Observer.Create(uowAdapter);
            handler.HandleDownstream(context, msg);


            storage.Received().Hold(uowMapper.GetBatchId(), msg.DomainEvent);
        }

        [Fact]
        public void UowDispatcher_UowReleasedSuccessfully()
        {
            var uowAdapter = new FakeUowAdapter();
            var storage = Substitute.For<IDomainEventStorage>();
            var context = Substitute.For<IDownstreamContext>();
            var msg = new DispatchEvent(new FakeEvent());
            var uowMapper = new ThreadBatchIdMapper();
            var handler = new TransactionalHandler(uowAdapter, storage, uowMapper);
            uowAdapter.Observer.Create(uowAdapter);
            var batchId = uowMapper.GetBatchId();
            storage.Release(batchId).Returns(new[] {new FakeEvent()});

            handler.HandleDownstream(context, msg);
            context.DidNotReceive().SendDownstream(msg);
            uowAdapter.Observer.Released(uowAdapter, true);


            storage.Received().Hold(batchId, msg.DomainEvent);
            storage.Received().Release(batchId);
            context.Received().SendDownstream(Arg.Any<DispatchEvent>());
        }


        [Fact]
        public void UowDispatcher_UowReleasedFailed()
        {
            var uowAdapter = new FakeUowAdapter();
            var storage = Substitute.For<IDomainEventStorage>();
            var context = Substitute.For<IDownstreamContext>();
            var msg = new DispatchEvent(new FakeEvent());
            var uowMapper = new ThreadBatchIdMapper();
            var handler = new TransactionalHandler(uowAdapter, storage, uowMapper);
            uowAdapter.Observer.Create(uowAdapter);
            var batchId = uowMapper.GetBatchId();
            storage.Release(batchId).Returns(new[] {new FakeEvent()});

            handler.HandleDownstream(context, msg);
            context.DidNotReceive().SendDownstream(msg);
            uowAdapter.Observer.Released(uowAdapter, false);


            storage.Received().Hold(batchId, msg.DomainEvent);
            storage.Received().Delete(batchId);
            context.DidNotReceive().SendDownstream(Arg.Any<DispatchEvent>());
        }
    }
}