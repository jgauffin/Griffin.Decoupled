using System;
using System.Linq;
using System.Threading;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands.Pipeline
{
    public class AsyncTests
    {
        [Fact]
        public void MustHaveTasks()
        {
            var storage = Substitute.For<ICommandStorage>();
            var inner = Substitute.For<ICommandDispatcher>();

            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncHandler(storage, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncHandler(storage, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncHandler(storage, 1000));
            new AsyncHandler(storage, 10);
        }

        [Fact]
        public void MustHaveStorage()
        {
            var storage = Substitute.For<ICommandStorage>();
            var inner = Substitute.For<ICommandDispatcher>();

            Assert.Throws<ArgumentNullException>(() => new AsyncHandler(null, 0));
            new AsyncHandler(storage, 10);
        }

        [Fact]
        public void Enqueue()
        {
            var storage = Substitute.For<ICommandStorage>();
            var dispatcher = new AsyncHandler(storage, 1);
            var context = new DownContext(null, null);
            var state = new DispatchCommand(new FakeCommand());
            storage.Dequeue().Returns(state);

            dispatcher.HandleDownstream(context, state);

            storage.Received().Add(state);
            Assert.True(context.WaitDown(TimeSpan.FromSeconds(1)));
            storage.Received().Dequeue();
        }

        [Fact]
        public void DispatchTwoSingleWorkers()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 1);
            var state1 = new DispatchCommand(new FakeCommand());
            var state2 = new DispatchCommand(new FakeCommand());
            var context = new DownContext(null, null);

            dispatcher.HandleDownstream(context, state1);
            dispatcher.HandleDownstream(context, state2);

            Assert.True(context.WaitDown(TimeSpan.FromSeconds(1)));
            Assert.Same(state2, storage.Dequeued.Last());
        }

        /*

        [Fact]
        public void Transaction_NoEntries()
        {
            var transaction = Substitute.For<ISimpleTransaction>();
            var storage = Substitute.For<ICommandStorage>();
            storage.BeginTransaction().Returns(transaction);
            storage.Dequeue().Returns((DispatchCommand)null);
            var context = new DownContext(null, null);

            var dispatcher = new AsyncHandler(storage, 1);
            dispatcher.HandleDownstream(context, new Started());
            Thread.Sleep(50); //only way I could think of to be able to wait on the worker.

            storage.Received().Dequeue();
            storage.Received().BeginTransaction();
            transaction.Received().Commit();
        }

        [Fact]
        public void Transaction_OneFailingEntry_ShouldRollback()
        {
            var transaction = Substitute.For<ISimpleTransaction>();
            var storage = Substitute.For<ICommandStorage>();
            storage.BeginTransaction().Returns(transaction);
            var command = new DispatchCommand(new FakeCommand());
            storage.Dequeue().Returns(command);
            var context = new DownContext(x => { if (!(x is DispatchCommand)) return; throw new Exception(); }, null);

            var dispatcher = new AsyncHandler(storage, 1);
            dispatcher.HandleDownstream(context, new Started());
            Thread.Sleep(50); //only way I could think of to be able to wait on the worker.

            storage.Received().Dequeue();
            storage.Received().BeginTransaction();
            transaction.DidNotReceive().Commit();
        }

        [Fact]
        public void Transaction_OneSuccessfulEntry_PipelineFailed()
        {
            var transaction = Substitute.For<ISimpleTransaction>();
            var storage = Substitute.For<ICommandStorage>();
            storage.BeginTransaction().Returns(transaction);
            var command = new DispatchCommand(new FakeCommand());
            storage.Dequeue().Returns(command);
            var context = new DownContext(x => { if (!(x is DispatchCommand)) return; throw new Exception(); }, null);

            var dispatcher = new AsyncHandler(storage, 1);
            dispatcher.HandleDownstream(context, new Started());
            Thread.Sleep(50); //only way I could think of to be able to wait on the worker.

            storage.Received().Dequeue();
            storage.Received().BeginTransaction();
            transaction.DidNotReceive().Commit();
        }

        [Fact]
        public void Transaction_OneSuccessfulEntry_Success()
        {
            var transaction = Substitute.For<ISimpleTransaction>();
            var storage = Substitute.For<ICommandStorage>();
            storage.BeginTransaction().Returns(transaction);
            var command = new DispatchCommand(new FakeCommand());
            storage.Dequeue().Returns(command);
            var context = new DownContext(null, null);

            var dispatcher = new AsyncHandler(storage, 1);
            dispatcher.HandleDownstream(context, new Started());
            Thread.Sleep(50); //only way I could think of to be able to wait on the worker.

            storage.Received().Dequeue();
            storage.Received().BeginTransaction();
            transaction.Received().Commit();
        }
        */

        [Fact]
        public void ShutDown_NoMoreDispatching()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 2);
            var command = new FakeCommand();
            var state = new DispatchCommand(command);
            var state2 = new DispatchCommand(new FakeCommand());
            var context = new DownContext(null, null);

            // dispatch first and check that it's passed by properly
            dispatcher.HandleDownstream(context, state);
            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(50)));
            context.ResetDown();

            dispatcher.Close();

            dispatcher.HandleDownstream(context, state2);
            Assert.False(context.WaitDown(TimeSpan.FromMilliseconds(50)));
        }


        [Fact]
        public void OnlyOneWorker_TriggerThroughQueue()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 1);
            var state1 = new DispatchCommand(new FakeCommand());
            var state2 = new DispatchCommand(new FakeCommand());
            var evt = new ManualResetEvent(false);
            var context = new DownContext(x => evt.WaitOne(), null);

            dispatcher.HandleDownstream(context, state1);
            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(50)));
            context.ResetDown();

            dispatcher.HandleDownstream(context, state2);
            Assert.False(context.WaitDown(TimeSpan.FromMilliseconds(50)));
            evt.Set();

            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(50)));
        }

        [Fact]
        public void TwoWorkers_DispatchTwoThreads()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 2);
            var state1 = new DispatchCommand(new FakeCommand());
            var state2 = new DispatchCommand(new FakeCommand());
            var evt = new ManualResetEvent(false);
            var context = new DownContext(null, x => evt.WaitOne());

            dispatcher.HandleDownstream(context, state1);
            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(100)));
            context.ResetDown();

            dispatcher.HandleDownstream(context, state2);
            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public void ExceptionHandler()
        {
            var sync = new ManualResetEvent(false);
            var expected = new Exception("Work not made");
            var dispatcher = new AsyncHandler(new MemoryStorage(), 2);
            object msg = null;
            var context = new DownContext(y => { throw expected; }, x =>
                {
                    msg = x;
                    sync.Set();
                });

            var state1 = new DispatchCommand(new FakeCommand());

            dispatcher.HandleDownstream(context, state1);

            Assert.True(sync.WaitOne(TimeSpan.FromMilliseconds(100)));
            Assert.NotNull(msg);
            Assert.IsType<PipelineFailure>(msg);
            Assert.Same(expected, ((PipelineFailure) msg).Exception);
        }
    }
}