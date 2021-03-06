﻿using System;
using Griffin.Decoupled.DomainEvents;
using Griffin.Decoupled.DomainEvents.Pipeline;
using Griffin.Decoupled.DomainEvents.Pipeline.Messages;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.Pipeline.Messages;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents.Pipeline
{
    public class IocHandlerTests
    {
        [Fact]
        public void IgnoreUnknownMessages()
        {
            var container = Substitute.For<IRootContainer>();
            var context = Substitute.For<IDownstreamContext>();

            var handler = new IocHandler(container);
            handler.HandleDownstream(context, Substitute.For<StartHandlers>());

            container.DidNotReceive().CreateScope();
            context.DidNotReceive().SendDownstream(Arg.Any<StartHandlers>());
        }

        [Fact]
        public void Throws()
        {
            var container = Substitute.For<IRootContainer>();
            var context = Substitute.For<IDownstreamContext>();
            var child = Substitute.For<IScopedContainer>();
            container.CreateScope().Returns(child);
            var msg = new DispatchEvent(new FakeEvent());
            child.ResolveAll<ISubscribeOn<FakeEvent>>().Returns(call => { throw new Exception(); });

            var handler = new IocHandler(container);
            handler.HandleDownstream(context, msg);

            context.Received().SendUpstream(Arg.Any<EventFailed>());
        }
    }
}