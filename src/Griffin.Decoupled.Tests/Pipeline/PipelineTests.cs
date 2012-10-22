using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Tests.Commands.Helpers;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Pipeline
{

    public class PipelineTests
    {
        [Fact]
        public void Destination_IsInvoked()
        {
            var handler = Substitute.For<IUpstreamHandler>();
            var downHandler = Substitute.For<IDownstreamHandler>();
            var pipeline = new Decoupled.Pipeline.Pipeline();
            var context = new DownstreamContext(downHandler);
            var msg = Substitute.For<IUpstreamMessage>();

            pipeline.Add(context);
            pipeline.Add(new UpstreamContext(handler));
            pipeline.Start();
            context.SendUpstream(msg);

            handler.Received().HandleUpstream(Arg.Any<IUpstreamContext>(), msg);
        }

        [Fact]
        public void DestinationIsNotSet()
        {
            var upContext = new UpstreamContext(new ForwardingUpHandler());
            var destination = Substitute.For<IUpstreamHandler>();
            var downHandler = Substitute.For<IDownstreamHandler>();
            var downContext = new DownstreamContext(downHandler);
            var pipeline = new Decoupled.Pipeline.Pipeline();
            var msg = Substitute.For<IUpstreamMessage>();

            pipeline.Add(downContext);
            pipeline.Add(upContext);
            pipeline.Add(new UpstreamContext(destination));
            pipeline.Start();
            upContext.Invoke(msg);

            destination.Received().HandleUpstream(Arg.Any<IUpstreamContext>(), msg);
        }

        [Fact]
        public void SpecifyAtLeastOneDownstream()
        {
            var destination = Substitute.For<IUpstreamHandler>();
            var pipeline = new Decoupled.Pipeline.Pipeline();

            Assert.Throws<InvalidOperationException>(() => pipeline.Start());
        }


        [Fact]
        public void NotStarted()
        {
            var pipeline = new Decoupled.Pipeline.Pipeline();
            Assert.Throws<InvalidOperationException>(() => pipeline.Send(new DispatchCommand(new FakeCommand())));
        }
    }
}
