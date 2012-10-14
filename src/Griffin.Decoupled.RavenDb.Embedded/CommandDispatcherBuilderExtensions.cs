using Griffin.Decoupled.Commands;
using Griffin.Decoupled.RavenDb.Commands;
using Raven.Client.Embedded;

namespace Griffin.Decoupled.RavenDb
{
    /// <summary>
    /// Extension methods for RavenDb &amp; The pipeline
    /// </summary>
    public static class CommandDispatcherBuilderExtensions
    {
        /// <summary>
        /// Use an embedded RavenDb database to store commands
        /// </summary>
        /// <param name="instance">this</param>
        /// <returns>this</returns>
        /// <remarks>You can configure the embedded DB manually like this:
        /// <code>
        /// // create raven
        /// var documentStore = new EmbeddableDocumentStore();
        /// documentStore.Initialize();
        /// 
        /// // assign it to the builder.
        /// pipelineBuilder.StoreCommands(new RavenCommandStorage(documentStore));
        /// </code>
        /// </remarks>
        public static PipelineDispatcherBuilder UseRavenDbEmbedded(this PipelineDispatcherBuilder instance)
        {
            var documentStore = new EmbeddableDocumentStore();
                /*{
                    //Conventions = {IdentityPartsSeparator = "-"},
                    //DefaultDatabase = "GriffinDecoupled"
                };*/
            //if (ConfigurationManager.ConnectionStrings["GriffinDecoupled"] != null)
            //documentStore.ConnectionStringName = "GriffinDecoupled";
            documentStore.Initialize();

            instance.StoreCommands(new RavenCommandStorage(documentStore));
            return instance;
        }
    }
}