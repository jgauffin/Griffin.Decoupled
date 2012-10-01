using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Commands
{
    public class CommandDispatcherBuilderTests
    {
        [Fact]
        public void NeedAnInner()
        {
            var builder = new CommandDispatcherBuilder();
            builder.MakeAsync(10, args => {});

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void AsyncRetrying()
        {
            var builder = new CommandDispatcherBuilder();
            builder.MakeAsync(10, args => {});
            builder.RetryCommands(4, args => {});

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Complete()
        {
            var builder = new CommandDispatcherBuilder();
            builder
                .MakeAsync(10, args => { })
                .RetryCommands(4, args => { })
                .Publisher(Substitute.For<ICommandDispatcher>())
                .Build();

            CommandDispatcher.Dispatch(new FakeCommand());
        }
    }
}
