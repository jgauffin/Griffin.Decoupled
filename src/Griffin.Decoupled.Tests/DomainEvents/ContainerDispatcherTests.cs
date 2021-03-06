﻿using Griffin.Decoupled.DomainEvents;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class ContainerDispatcherTests
    {
        [Fact]
        public void Dispatch()
        {
            var root = Substitute.For<IRootContainer>();
            var child = Substitute.For<IScopedContainer>();
            var handler = Substitute.For<ISubscribeOn<FakeEvent>>();
            root.CreateScope().Returns(child);
            child.ResolveAll<ISubscribeOn<FakeEvent>>().Returns(new[] {handler});
            var theEvent = new FakeEvent();

            var dispatcher = new IocDispatcher(root);
            dispatcher.Dispatch(theEvent);

            handler.Received().Handle(theEvent);
        }
    }
}