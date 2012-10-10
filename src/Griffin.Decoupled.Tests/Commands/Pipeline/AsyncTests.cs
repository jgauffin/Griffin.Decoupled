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
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncHandler(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncHandler(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncHandler(1000));
            new AsyncHandler(10);
        }

        [Fact]
        public void Enqueue()
        {
            var dispatcher = new AsyncHandler(1);
            var context = new DownContext(null, null);
            var state = new DispatchCommand(new FakeCommand());

            dispatcher.HandleDownstream(context, state);

            Assert.True(context.WaitDown(TimeSpan.FromSeconds(1)));
            Assert.Same(state, context.Message);
        }

        [Fact]
        public void DispatchTwo_SingleWorker()
        {
            var dispatcher = new AsyncHandler(1);
            var state1 = new DispatchCommand(new FakeCommand());
            var state2 = new DispatchCommand(new FakeCommand());
            var context = new DownContext(null, null);

            dispatcher.HandleDownstream(context, state1);
            dispatcher.HandleDownstream(context, state2);

            Assert.True(context.WaitDown(TimeSpan.FromSeconds(1)));
            Assert.IsType<DispatchCommand>(context.Message);
        }

        [Fact]
        public void ShutDown_NoMoreDispatching()
        {
            var dispatcher = new AsyncHandler(2);
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
            var dispatcher = new AsyncHandler(1);
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
            var dispatcher = new AsyncHandler(2);
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
            var dispatcher = new AsyncHandler(2);
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