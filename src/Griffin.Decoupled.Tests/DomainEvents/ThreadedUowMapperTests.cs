﻿using System;
using Griffin.Decoupled.DomainEvents;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class ThreadedUowMapperTests
    {
        [Fact]
        public void Create()
        {
            var mapper = new ThreadBatchIdMapper();

            mapper.Create(mapper);
        }

        [Fact]
        public void CreateNull()
        {
            var mapper = new ThreadBatchIdMapper();
            Assert.Throws<ArgumentNullException>(() => mapper.Create(null));
        }

        [Fact]
        public void CreateAndGet()
        {
            var mapper = new ThreadBatchIdMapper();
            var guid = mapper.Create(mapper);

            var result = mapper.Release(mapper);

            Assert.Equal(guid, result);
        }

        [Fact]
        public void CreateTwo_ReleaseOne()
        {
            var mapper = new ThreadBatchIdMapper();
            mapper.Create(mapper);
            mapper.Create(this);

            mapper.Release(mapper);

            Assert.True(mapper.IsActive);
        }

        [Fact]
        public void CreateTwo_ReleaseTwo()
        {
            var mapper = new ThreadBatchIdMapper();
            mapper.Create(mapper);
            mapper.Create(this);

            mapper.Release(mapper);
            mapper.Release(this);

            Assert.False(mapper.IsActive);
        }

        [Fact]
        public void CreateTwo_ReleaseUnknown()
        {
            var mapper = new ThreadBatchIdMapper();
            mapper.Create(mapper);
            mapper.Create(this);

            Assert.Throws<InvalidOperationException>(() => mapper.Release(new object()));
        }

        [Fact]
        public void CreateNone_ReleaseUnknown()
        {
            var mapper = new ThreadBatchIdMapper();

            Assert.Throws<InvalidOperationException>(() => mapper.Release(new object()));
        }

        [Fact]
        public void IsActive()
        {
            var mapper = new ThreadBatchIdMapper();
            var guid = mapper.Create(mapper);

            Assert.True(mapper.IsActive);
        }

        [Fact]
        public void IsNotActive()
        {
            var mapper = new ThreadBatchIdMapper();

            Assert.False(mapper.IsActive);
        }
    }
}