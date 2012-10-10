using System;
using System.Linq;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands
{
    public class RetryingDispatcherTests
    {
        [Fact]
        public void InvalidAttemptCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingHandler(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingHandler(100));
            new RetryingHandler(10);
        }

        [Fact]
        public void FailAll()
        {
            var dispatcher = new RetryingHandler(3);
            var msg = new CommandFailed(new DispatchCommand(new FakeCommand(), 3), new Exception());
            var context = Substitute.For<IUpstreamContext>();

            dispatcher.HandleUpstream(context, msg);

            context.Received().SendUpstream(Arg.Any<CommandAborted>());
        }

        [Fact]
        public void FailTwo()
        {
            var context = Substitute.For<IUpstreamContext>();
            var msg = new CommandFailed(new DispatchCommand(new FakeCommand(), 1), new Exception());
            var dispatcher = new RetryingHandler(3);

            dispatcher.HandleUpstream(context, msg);

            context.Received().SendUpstream(Arg.Any<CommandFailed>());
        }

        [Fact]
        public void PassThrough()
        {
            var context = Substitute.For<IUpstreamContext>();
            var state = new DispatchCommand(new FakeCommand(), 3);
            var dispatcher = new RetryingHandler(3);

            dispatcher.HandleUpstream(context, state);

            context.Received().SendUpstream(Arg.Any<DispatchCommand>());
        }
    }
}