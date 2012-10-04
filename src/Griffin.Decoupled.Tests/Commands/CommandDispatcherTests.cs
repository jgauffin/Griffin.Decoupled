using System;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands
{
    public class CommandDispatcherTests
    {
        [Fact]
        public void RegularDispatch()
        {
            var innerDispatcher = Substitute.For<ICommandDispatcher>();
            CommandDispatcher.Assign(innerDispatcher);
            var command = new FakeCommand();

            CommandDispatcher.Dispatch(command);

            innerDispatcher.Received().Dispatch(Arg.Is<FakeCommand>(x => ReferenceEquals(x, command)));
        }

        [Fact]
        public void NoDispatcher()
        {
            CommandDispatcher.Assign(null);
            var command = Substitute.For<ICommand>();

            Assert.Throws<InvalidOperationException>(() => CommandDispatcher.Dispatch(command));
        }
    }
}