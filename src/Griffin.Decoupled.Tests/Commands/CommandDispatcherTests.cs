using System;
using Griffin.Decoupled.Commands;
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
            var command = Substitute.For<ICommand>();

            CommandDispatcher.Dispatch(command);

            innerDispatcher.Received().Dispatch(command);
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