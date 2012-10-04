using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands
{
    public class AsyncDispatcherTests
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
            var state = new SendCommand(new FakeCommand());
            storage.Dequeue().Returns( state);

            dispatcher.HandleDownstream(context, state);

            storage.Received().Enqueue( state);
            Assert.True(context.WaitDown(TimeSpan.FromSeconds(1)));
            storage.Received().Dequeue();
        }

        [Fact]
        public void DispatchTwoSingleWorker()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 1);
            var state1 = new SendCommand(new FakeCommand());
            var state2 = new SendCommand(new FakeCommand());
            var context = new DownContext(null, null);

            dispatcher.HandleDownstream(context, state1);
            dispatcher.HandleDownstream(context, state2);

            Assert.True(context.WaitDown(TimeSpan.FromSeconds(1)));
            Assert.Same( state2, storage.Dequeued.Last());
        }



        [Fact]
        public void ShutDown_NoMoreDispatching()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 2);
            var command = new FakeCommand();
            var state = new SendCommand(command);
            var state2 = new SendCommand(new FakeCommand());
            var context = new DownContext(null, null);

            // dispatch first and check that it's passed by properly
            dispatcher.HandleDownstream(context, state);
            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(200)));
            context.ResetDown();

            dispatcher.Close();

            dispatcher.HandleDownstream(context, state2);
            Assert.False(context.WaitDown(TimeSpan.FromMilliseconds(200)));
        }


        [Fact]
        public void OnlyOneWorker_TriggerThroughQueue()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 1);
            var state1 = new SendCommand(new FakeCommand());
            var state2 = new SendCommand(new FakeCommand());
            var evt = new ManualResetEvent(false);
            var context = new DownContext(x=>evt.WaitOne(), null);

            dispatcher.HandleDownstream(context, state1);
            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(100)));
            context.ResetDown();

            dispatcher.HandleDownstream(context, state2);
            Assert.False(context.WaitDown(TimeSpan.FromMilliseconds(100)));
            evt.Set();

            Assert.True(context.WaitDown(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public void TwoWorkers_DispatchTwoThreads()
        {
            var storage = new TestStorage();
            var dispatcher = new AsyncHandler(storage, 2);
            var state1 = new SendCommand(new FakeCommand());
            var state2 = new SendCommand(new FakeCommand());
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

            var state1 = new SendCommand(new FakeCommand());

            dispatcher.HandleDownstream(context, state1);

            Assert.True(sync.WaitOne(TimeSpan.FromMilliseconds(100)));
            Assert.NotNull(msg);
            Assert.IsType<PipelineFailure>(msg);
            Assert.Same(expected, ((PipelineFailure)msg).Exception);
        }


    }
}
