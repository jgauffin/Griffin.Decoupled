using Griffin.Decoupled.Commands;
using Raven.Client.Embedded;

namespace Griffin.Decoupled.RavenDb
{
    public static class CommandDispatcherBuilderExtensions
    {
        public static PipelineDispatcherBuilder UseRavenDbEmbedded(this PipelineDispatcherBuilder instance,
                                                                   bool useTransactions = true)
        {
            var documentStore = new EmbeddableDocumentStore
                {
                    //Conventions = {IdentityPartsSeparator = "-"},
                    //DefaultDatabase = "GriffinDecoupled"
                };
            //if (ConfigurationManager.ConnectionStrings["GriffinDecoupled"] != null)
            //documentStore.ConnectionStringName = "GriffinDecoupled";
            documentStore.Initialize();

            instance.StoreCommands(new RavenCommandStorage(documentStore));
            return instance;
        }
    }
}