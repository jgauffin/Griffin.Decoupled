using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands
{
    public class ContainerDispatcherTests
    {
        [Fact]
        public void Dispatch()
        {
            var root = Substitute.For<IRootContainer>();
            var child = Substitute.For<IScopedContainer>();
            var handler = Substitute.For<IHandleCommand<FakeCommand>>();
            root.CreateScope().Returns(child);
            child.ResolveAll<IHandleCommand<FakeCommand>>().Returns(new[] {handler});
            var command = new FakeCommand();

            var dispatcher = new ContainerCommandDispatcher(root);
            dispatcher.Dispatch(new CommandState(command));

            handler.Received().Invoke(command);
        }
    }
}