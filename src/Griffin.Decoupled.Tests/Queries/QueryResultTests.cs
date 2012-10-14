using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Queries;
using Xunit;

namespace Griffin.Decoupled.Tests.Queries
{
    public class QueryResultTests
    {
        [Fact]
        public void NoItems()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryResult<FakeAggregate>(null, 5));
        }
        [Fact]
        public void MinusCount()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryResult<FakeAggregate>(null, -1));
        }

        [Fact]
        public void AssignedCorrectly()
        {
            var items = new FakeAggregate[] {new FakeAggregate(), new FakeAggregate()};
            var conditions = new QueryResult<FakeAggregate>(items, 10);

            Assert.Same(items, conditions.Items);
            Assert.Equal(10, conditions.TotalCount);
        }

    }
}
