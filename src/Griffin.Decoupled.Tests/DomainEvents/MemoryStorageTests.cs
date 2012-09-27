using System;
using System.Linq;
using Griffin.Decoupled.DomainEvents;
using Griffin.Decoupled.Tests.DomainEvents.Helpers;
using Xunit;

namespace Griffin.Decoupled.Tests.DomainEvents
{
    public class MemoryStorageTests
    {
        [Fact]
        public void LoadNoId()
        {
            var storage = new MemoryStorage();

            Assert.Throws<ArgumentException>(() => storage.Load(Guid.Empty));
        }

        [Fact]
        public void LoadUnknownId()
        {
            var storage = new MemoryStorage();

            var result = storage.Load(Guid.NewGuid());

            Assert.Empty(result);
        }

        [Fact]
        public void StoreEmptyId()
        {
            var theEvent = new FakeEvent();
            var storage = new MemoryStorage();

            Assert.Throws<ArgumentException>(() => storage.Store(Guid.Empty, theEvent));
        }


        [Fact]
        public void LoadCorrectId()
        {
            var guid = Guid.NewGuid();
            var theEvent = new FakeEvent();

            var storage = new MemoryStorage();
            storage.Store(guid, theEvent);
            var result = storage.Load(guid);

            Assert.Same(theEvent, result.FirstOrDefault());
        }

        [Fact]
        public void DeleteCorrectId()
        {
            var guid = Guid.NewGuid();
            var theEvent = new FakeEvent();

            var storage = new MemoryStorage();
            storage.Store(guid, theEvent);
            storage.Delete(guid);
        }

        [Fact]
        public void DeleteUnknown()
        {
            var storage = new MemoryStorage();
            Assert.Throws<InvalidOperationException>(() => storage.Delete(Guid.NewGuid()));
        }

        [Fact]
        public void DeleteEmptyId()
        {
            var storage = new MemoryStorage();
            Assert.Throws<ArgumentException>(() => storage.Delete(Guid.Empty));
        }
    }
}