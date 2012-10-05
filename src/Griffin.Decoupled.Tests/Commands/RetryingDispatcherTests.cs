using System;
using System.Linq;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
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
            var storage = new TestStorage();

            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingHandler(0, storage));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RetryingHandler(100, storage));
            new RetryingHandler(10, storage);
        }

        [Fact]
        public void FailAll()
        {
            var storage = Substitute.For<ICommandStorage>();
            var dispatcher = new RetryingHandler(3, storage);
            var msg = new CommandFailed(new SendCommand(new FakeCommand(), 3), new Exception());
            var context = Substitute.For<IUpstreamContext>();

            dispatcher.HandleUpstream(context, msg);

            context.Received().SendUpstream(Arg.Any<CommandAborted>());
            storage.Received().Delete(msg.Message.Command);
        }

        [Fact]
        public void FailTwo()
        {
            var context = Substitute.For<IUpstreamContext>();
            var storage = Substitute.For<ICommandStorage>();
            var msg = new CommandFailed(new SendCommand(new FakeCommand(), 1), new Exception());
            var dispatcher = new RetryingHandler(3, storage);

            dispatcher.HandleUpstream(context, msg);

            context.Received().SendUpstream(Arg.Any<CommandFailed>());
            storage.Received().Update(msg.Message);
        }

        [Fact]
        public void PassThrough()
        {
            var context = Substitute.For<IUpstreamContext>();
            var storage = Substitute.For<ICommandStorage>();
            var state = new SendCommand(new FakeCommand(), 3);
            var dispatcher = new RetryingHandler(3, storage);

            dispatcher.HandleUpstream(context, state);

            context.Received().SendUpstream(Arg.Any<SendCommand>());
            Assert.Equal(0, storage.ReceivedCalls().Count());
        }
    }
}