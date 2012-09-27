using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            DomainEvent.Dispatch(domainEvent);

            innerDispatcher.Received().Dispatch(domainEvent);
        }

        [Fact]
        public void NoDispatcher()
        {
            DomainEvent.Assign(null);
            var domainEvent = Substitute.For<IDomainEvent>();

            Assert.Throws<InvalidOperationException>(() => DomainEvent.Dispatch(domainEvent));
        }
    }

}
