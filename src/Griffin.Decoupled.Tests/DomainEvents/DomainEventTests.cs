using System;
using Griffin.Decoupled.DomainEvents;
using NSubstitute;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class DomainEventTests
    {
        [Fact]
        public void RegularDispatch()
        {
            var innerDispatcher = Substitute.For<IDomainEventDispatcher>();
            DomainEvent.Assign(innerDispatcher);
            var domainEvent = Substitute.For<IDomainEvent>();

            DomainEvent.Publish(domainEvent);

            innerDispatcher.Received().Dispatch(domainEvent);
        }

        [Fact]
        public void NoDispatcher()
        {
            DomainEvent.Assign(null);
            var domainEvent = Substitute.For<IDomainEvent>();

            Assert.Throws<InvalidOperationException>(() => DomainEvent.Publish(domainEvent));
        }
    }
}