using System;
using System.Linq;
using System.Threading;
using Griffin.Decoupled.DomainEvents;
using Griffin.Decoupled.DomainEvents.Pipeline;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class DefaultDispatcherTests
    {
        [Fact]
        public void RegularDispatch()
        {
            var dispatcher = new AsyncHandler(5);
            var msg = new DispatchEvent(new FakeEvent());
            var context = new FakeContext();

            dispatcher.HandleDownstream(context, msg);
            
            Assert.True(context.Wait(TimeSpan.FromMilliseconds(500)));
            Assert.Same(msg, context.Message);
        }


    }
}