using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Tests.Commands.Helpers;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.Pipeline
{
    public class PipelineBuilderTests
    {
        [Fact]
        public void Test()
        {
            var down1 = new ForwardingDownHandler();
            var down2 = new ForwardingDownHandler();
            var up1 = new ForwardingUpHandler();
            var up2 = new ForwardingUpHandler();
            var handler = Substitute.For<IUpstreamHandler>();

            var bp = new PipelineBuilder();
            bp.RegisterDownstream(down1);
            bp.RegisterDownstream(down2);
            bp.RegisterUpstream(up1);
            bp.RegisterUpstream(up2);
            bp.RegisterUpstream(handler);
            var pipeline = bp.Build();

            pipeline.Start();
            pipeline.Send(new DispatchEvent(new FakeEvent()));

            foreach (var call in handler.ReceivedCalls())
            {
                Console.WriteLine(call.GetArguments()[1]);
            }
        }
    }
}
