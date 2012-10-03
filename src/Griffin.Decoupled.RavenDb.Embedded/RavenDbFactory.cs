using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Decoupled.Commands;
using Raven.Client.Embedded;

namespace Griffin.Decoupled.RavenDb
{


    public static class CommandDispatcherBuilderExtensions
    {
        public static CommandDispatcherBuilder StoreCommandsInRavenDbEmbedded(this CommandDispatcherBuilder instance, bool useTransactions = true)
        {
            var documentStore = new EmbeddableDocumentStore
                {
                    Conventions = { IdentityPartsSeparator = "-" },
                    DefaultDatabase = "GriffinDecoupled"
                };
            if (ConfigurationManager.ConnectionStrings["GriffinDecoupled"] != null)
                documentStore.ConnectionStringName = "GriffinDecoupled";
            documentStore.Initialize();

            instance.StoreCommands(new RavenCommandStorage(documentStore.OpenSession()));
            return instance;
        }
    }
}
