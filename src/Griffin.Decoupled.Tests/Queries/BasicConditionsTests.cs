using System;
using Griffin.Decoupled.Queries;
using Xunit;

namespace Griffin.Decoupled.Tests.Queries
{
    public class BasicConditionsTests
    {
        [Fact]
        public void UnknownProperty()
        {
            var conditions = new BasicConditions<FakeAggregate, FakeQuery>();
            Assert.Throws<ArgumentException>(() => conditions.SortBy("Mamma"));
        }


        [Fact]
        public void SortBy()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            constraints.SortBy("DisplayName");

            Assert.Equal(SortOrder.Ascending, constraints.SortOrder);
            Assert.Equal("DisplayName", constraints.SortPropertyName);
        }

        [Fact]
        public void SortByDescending()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            constraints.SortByDescending("DisplayName");

            Assert.Equal(SortOrder.Descending, constraints.SortOrder);
            Assert.Equal("DisplayName", constraints.SortPropertyName);
        }

        [Fact]
        public void NoPaging()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            Assert.Equal(-1, constraints.PageNumber);
            Assert.Equal(-1, constraints.PageSize);
        }

        [Fact]
        public void NoSorting()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            Assert.Equal(null, constraints.SortPropertyName);
        }

        [Fact]
        public void FirstPage()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            constraints.Page(1, 50);

            Assert.Equal(1, constraints.PageNumber);
            Assert.Equal(50, constraints.PageSize);
        }


        [Fact]
        public void TenthPage()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            constraints.Page(10, 20);

            Assert.Equal(10, constraints.PageNumber);
            Assert.Equal(20, constraints.PageSize);
        }

        [Fact]
        public void TypedSortBy()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            constraints.SortBy(x => x.DisplayName);

            Assert.Equal(SortOrder.Ascending, constraints.SortOrder);
            Assert.Equal("DisplayName", constraints.SortPropertyName);
        }

        [Fact]
        public void TypedSortByDescending()
        {
            var constraints = new BasicConditions<FakeAggregate, FakeQuery>();

            constraints.SortByDescending(x => x.DisplayName);

            Assert.Equal(SortOrder.Descending, constraints.SortOrder);
            Assert.Equal("DisplayName", constraints.SortPropertyName);
        }
    }
}