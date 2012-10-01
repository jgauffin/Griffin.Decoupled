using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
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

            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncDispatcher(inner, storage, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncDispatcher(inner, storage, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncDispatcher(inner, storage, 1000));
            new AsyncDispatcher(inner, storage, 10);
        }

        [Fact]
        public void MustHaveDispatcher()
        {
            var storage = Substitute.For<ICommandStorage>();
            var inner = Substitute.For<ICommandDispatcher>();

            Assert.Throws<ArgumentNullException>(() => new AsyncDispatcher(null, storage, 0));
            new AsyncDispatcher(inner, storage, 10);
        }
        [Fact]
        public void MustHaveStorage()
        {
            var storage = Substitute.For<ICommandStorage>();
            var inner = Substitute.For<ICommandDispatcher>();

            Assert.Throws<ArgumentNullException>(() => new AsyncDispatcher(inner, null, 0));
            new AsyncDispatcher(inner, storage, 10);
        }

        [Fact]
        public void Enqueue()
        {
            var storage = Substitute.For<ICommandStorage>();
            var inner = new BlockingDispatcher();
            var dispatcher = new AsyncDispatcher(inner, storage, 1);
            var command = new FakeCommand();
            var state = new CommandState(command);
            storage.Dequeue().Returns(state);

            dispatcher.Dispatch(state);

            storage.Received().Enqueue(state);
            Assert.True(inner.Wait(TimeSpan.FromSeconds(1)));
            storage.Received().Dequeue();
        }

        [Fact]
        public void DispatchTwoSingleWorker()
        {
            var storage = new TestStorage();
            var inner = new BlockingDispatcher(2);
            var dispatcher = new AsyncDispatcher(inner, storage, 1);
            var state1 = new CommandState(new FakeCommand());
            var state2 = new CommandState(new FakeCommand());

            dispatcher.Dispatch(state1);
            dispatcher.Dispatch(state2);

            Assert.True(inner.Wait(TimeSpan.FromSeconds(1)));
            Assert.Same(state2, storage.Dequeued.Last());
        }



        [Fact]
        public void ShutDown_NoMoreDispatching()
        {
            var storage = new TestStorage();
            var inner = new BlockingDispatcher();
            var dispatcher = new AsyncDispatcher(inner, storage, 2);
            var command = new FakeCommand();
            var state = new CommandState(command);
            var state2 = new CommandState(new FakeCommand());

            // dispatch first and check that it's passed by properly
            dispatcher.Dispatch(state);
            Assert.True(inner.Wait(TimeSpan.FromMilliseconds(200)));
            inner.Reset();

            dispatcher.Close();

            dispatcher.Dispatch(state2);
            Assert.False(inner.Wait(TimeSpan.FromMilliseconds(200)));


        }


        [Fact]
        public void OnlyOneWorker_TriggerThroughQueue()
        {
            var storage = new TestStorage();
            var inner = new BlockingDispatcher();
            var dispatcher = new AsyncDispatcher(inner, storage, 1);
            var state1 = new CommandState(new FakeCommand());
            var state2 = new CommandState(new FakeCommand());

            inner.BlockDispatchInvocation();
            dispatcher.Dispatch(state1);
            Assert.True(inner.Wait(TimeSpan.FromMilliseconds(100)));
            inner.Reset();

            dispatcher.Dispatch(state2);
            Assert.False(inner.Wait(TimeSpan.FromMilliseconds(100)));
            inner.UnblockDispatchInvocation();

            Assert.True(inner.Wait(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public void TwoWorkers_DispatchTwoThreads()
        {
            var storage = new TestStorage();
            var inner = new BlockingDispatcher();
            var dispatcher = new AsyncDispatcher(inner, storage, 2);
            var state1 = new CommandState(new FakeCommand());
            var state2 = new CommandState(new FakeCommand());

            inner.BlockDispatchInvocation();
            dispatcher.Dispatch(state1);
            Assert.True(inner.Wait(TimeSpan.FromMilliseconds(100)));
            inner.Reset();

            dispatcher.Dispatch(state2);
            Assert.True(inner.Wait(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public void ExceptionHandler()
        {
            var sync = new ManualResetEvent(false);
            var expected = new Exception("Work not made");
            var inner = new BlockingDispatcher(x => { throw expected; });
            var dispatcher = new AsyncDispatcher(inner, 2);
            Exception actual = null;
            dispatcher.UncaughtException += (sender, args) =>
                {
                    actual = args.Exception;
                    sync.Set();
                };
            var state1 = new CommandState(new FakeCommand());

            dispatcher.Dispatch(state1);

            Assert.True(sync.WaitOne(TimeSpan.FromMilliseconds(100)));
            Assert.Same(expected, actual);
        }


    }
}
