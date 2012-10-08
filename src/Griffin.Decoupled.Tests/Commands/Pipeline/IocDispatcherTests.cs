using System;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;
using IocDispatcher = Griffin.Decoupled.Commands.Pipeline.IocDispatcher;

namespace Griffin.Decoupled.Tests.Commands.Pipeline
{
    public class IocDispatcherTests
    {
        [Fact]
        public void Dispatch()
        {
            var root = Substitute.For<IRootContainer>();
            var child = Substitute.For<IScopedContainer>();
            var handler = Substitute.For<IHandleCommand<FakeCommand>>();
            root.CreateScope().Returns(child);
            child.Resolve<IHandleCommand<FakeCommand>>().Returns(handler);
            var context = Substitute.For<IDownstreamContext>();
            var storage = Substitute.For<ICommandStorage>();
            var dispatcher = new IocDispatcher(root, storage);
            var msg = new DispatchCommand(new FakeCommand());

            dispatcher.HandleDownstream(context, msg);

            handler.Received().Invoke((FakeCommand) msg.Command);
            context.DidNotReceive().SendUpstream(Arg.Any<CommandFailed>());
            storage.Received().Delete(msg.Command);
        }

        [Fact]
        public void CommandFailed_SendUpstream()
        {
            var root = Substitute.For<IRootContainer>();
            var child = Substitute.For<IScopedContainer>();
            var handler = new BlockingHandler<FakeCommand>(x => { throw new Exception(); });
            root.CreateScope().Returns(child);
            child.Resolve<IHandleCommand<FakeCommand>>().Returns(handler);
            var context = Substitute.For<IDownstreamContext>();
            var storage = Substitute.For<ICommandStorage>();
            var dispatcher = new IocDispatcher(root, storage);
            var msg = new DispatchCommand(new FakeCommand());

            dispatcher.HandleDownstream(context, msg);

            context.Received().SendUpstream(Arg.Any<CommandFailed>());
            Assert.Equal(1, msg.Attempts);
        }
    }
}