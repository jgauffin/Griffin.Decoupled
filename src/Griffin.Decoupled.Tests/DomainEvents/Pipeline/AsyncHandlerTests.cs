using System;
using System.Threading;
using Griffin.Decoupled.DomainEvents.Pipeline;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents.Pipeline
{
    public class AsyncHandlerTests
    {
        [Fact]
        public void RegularDispatch()
        {
            var dispatcher = new AsyncHandler(5);
            var msg = new DispatchEvent(new FakeEvent());
            var context = new FakeContext();

            dispatcher.HandleDownstream(context, msg);

            Assert.True(context.Wait(TimeSpan.FromMilliseconds(50000)));
            Assert.Same(msg, context.Message);
        }

        [Fact]
        public void WorkersBusy()
        {
            var msg1 = new DispatchEvent(new FakeEvent());
            var msg2 = new DispatchEvent(new FakeEvent());
            var msg3 = new DispatchEvent(new FakeEvent());
            var evt = new ManualResetEvent(false);
            var context = new FakeContext(X => evt.WaitOne());
            var dispatcher = new AsyncHandler(2);

            // make first worker busy and validate
            dispatcher.HandleDownstream(context, msg1);
            Assert.True(context.Wait(TimeSpan.FromMilliseconds(100)));
            context.Reset();

            // make second worker busy and validate
            dispatcher.HandleDownstream(context, msg2);
            Assert.True(context.Wait(TimeSpan.FromMilliseconds(100)));
            context.Reset();

            // Make sure that the third message is not dispatched.
            dispatcher.HandleDownstream(context, msg3);
            Assert.False(context.Wait(TimeSpan.FromMilliseconds(100)));
            context.Reset();

            // release workers.
            evt.Set();

            // and expect the third to get running.
            Assert.True(context.Wait(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public void Shutdown_NotReleased()
        {
            var msg1 = new DispatchEvent(new FakeEvent());
            var evt = new ManualResetEvent(false);
            var context = new FakeContext(X => evt.WaitOne());
            var dispatcher = new AsyncHandler(2);

            dispatcher.HandleDownstream(context, msg1);
            Assert.Throws<InvalidOperationException>(() => dispatcher.Close());

            evt.Set();

            // and expect the third to get running.
            Assert.True(context.Wait(TimeSpan.FromMilliseconds(100)));
        }

        [Fact]
        public void Shutdown_ReleaseAfter1Second()
        {
            var msg1 = new DispatchEvent(new FakeEvent());
            var evt = new ManualResetEvent(false);
            var context = new FakeContext(X => Thread.Sleep(1000));
            var dispatcher = new AsyncHandler(2);

            dispatcher.HandleDownstream(context, msg1);
            dispatcher.Close();
        }
    }
}