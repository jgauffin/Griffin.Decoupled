using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Tests.Commands.Helpers;
using NSubstitute;
using Xunit;
using PipelineBuilder = Griffin.Decoupled.Commands.PipelineBuilder;

namespace Griffin.Decoupled.Tests.Commands
{
    public class CommandDispatcherBuilderTests
    {
        [Fact]
        public void NeedAnInner()
        {
            var builder = new PipelineBuilder(Substitute.For<IUpstreamHandler>());
            builder.AsyncDispatching(10);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void AsyncRetrying()
        {
            var builder = new PipelineBuilder(Substitute.For<IUpstreamHandler>());
            builder.AsyncDispatching(10);
            builder.RetryCommands(4);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Complete()
        {
            var builder = new PipelineBuilder(Substitute.For<IUpstreamHandler>());
            builder
                .AsyncDispatching(10)
                .RetryCommands(4)
                .Dispatcher(Substitute.For<IDownstreamHandler>())
                .Build();

            CommandDispatcher.Dispatch(new FakeCommand());
        }
    }
}
