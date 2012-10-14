using System;
using System.Linq;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.RavenDb.Commands;
using Raven.Client;
using Raven.Client.Embedded;
using Xunit;

namespace Griffin.Decoupled.RavenDb.Embedded.Tests
{
    public class CommandStorageTests
    {
        private readonly EmbeddableDocumentStore _store;

        public CommandStorageTests()
        {
            _store = new EmbeddableDocumentStore();
            // _store.DataDirectory = Environment.CurrentDirectory + "Db";
            _store.RunInMemory = true;
            _store.Initialize();
        }

        private IDocumentSession CreateSession()
        {
            return _store.OpenSession();
        }


        [Fact]
        public void Test()
        {
            var obj = new StoredCommand(new DispatchCommand(new TempCommand()));
            using (var session = _store.OpenSession())
            {
                session.Store(obj);
                session.SaveChanges();
            }
            using (var session = _store.OpenSession())
            {
                Assert.NotNull(session.Query<StoredCommand>().FirstOrDefault());
            }
            using (var session = _store.OpenSession())
            {
                var item = session.Load<StoredCommand>(obj.Id);
                session.Delete(item);
            }
        }

        [Fact]
        public void Store()
        {
            var commandStore = new RavenCommandStorage(_store);
            /*
            var item = commandStore.Dequeue();
            while (item != null)
            {
                commandStore.Delete(item.Command);
                item = commandStore.Dequeue();
            }
            */
            commandStore.Add(new DispatchCommand(new TempCommand()));

            //Assert.NotNull(commandStore.Dequeue());
        }

        #region Nested type: SomeTest

        public class SomeTest
        {
            public SomeTest()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; private set; }
            public ICommand Command { get; set; }
        }

        #endregion
    }

    public class TempCommand : CommandBase
    {
    }
}