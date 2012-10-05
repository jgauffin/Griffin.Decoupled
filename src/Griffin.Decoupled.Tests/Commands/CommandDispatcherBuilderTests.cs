using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Pipeline;
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
            var builder = new PipelineDispatcherBuilder(Substitute.For<IUpstreamHandler>());
            builder.AsyncDispatching(10);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void AsyncRetrying()
        {
            var builder = new PipelineDispatcherBuilder(Substitute.For<IUpstreamHandler>());
            builder.AsyncDispatching(10);
            builder.RetryCommands(4);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Complete()
        {
            var builder = new PipelineDispatcherBuilder(Substitute.For<IUpstreamHandler>());
            var dispatcher = builder
                .AsyncDispatching(10)
                .RetryCommands(4)
                .Dispatcher(Substitute.For<IDownstreamHandler>())
                .Build();
            CommandDispatcher.Assign(dispatcher);

            CommandDispatcher.Dispatch(new FakeCommand());
        }
    }
}
