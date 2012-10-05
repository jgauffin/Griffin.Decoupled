using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
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
            var command = new FakeCommand();
            var context = new DownContext(null, null);
            var dispatcher = new IocDispatcher(root);

            dispatcher.HandleDownstream(context, new SendCommand(command));

            handler.Received().Invoke(command);
        }
    }
}