using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Pipeline;
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
            var pipeline = new Griffin.Decoupled.Pipeline.Pipeline();
            var context = new DownstreamContext(downHandler);
            var msg = Substitute.For<IUpstreamMessage>();

            pipeline.Add(context);
            pipeline.SetDestination(handler);
            pipeline.Start();
            context.SendUpstream(msg);

            handler.Received().HandleUpstream(Arg.Any<IUpstreamContext>(), msg);
        }
    }
}
