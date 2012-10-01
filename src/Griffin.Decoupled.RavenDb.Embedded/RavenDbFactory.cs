using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Raven.Client.Embedded;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Will create a session factory for ravenDb using the embedded engine
    /// </summary>
    public class RavenDbFactory
    {
        private EmbeddableDocumentStore _documentStore;

        public RavenDbFactory()
        {
            _documentStore = new EmbeddableDocumentStore { ConnectionStringName = "GriffinDecoupled" };
            _documentStore.Conventions.IdentityPartsSeparator = "-";
            _documentStore.Initialize();

        }

        /// <summary>
        /// Create a new session factory.
        /// </summary>
        /// <returns></returns>
        public RavenSessionFactory CreateSessionFactory()
        {
            return new RavenSessionFactory(_documentStore);
        }
    }



    public static class CommandDispatcherBuilderExtensions
    {
        public static CommandDispatcherBuilder StoreInRavenDbEmbedded(this CommandDispatcherBuilder instance)
        {
            //instance.StoreCommands(new RavenCommandStorage())
            return null;
        }
    }
}
