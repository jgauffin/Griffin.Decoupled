using System;
using System.Reflection;
using Griffin.Container;
using Griffin.Decoupled;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Pipeline;
using Griffin.Decoupled.RavenDb;

namespace Sample_1___Hello_domain_event
{
    public class SayHello : CommandBase
    {
    }

    [Component]
    public class SayHelloHandler : IHandleCommand<SayHello>
    {
        #region IHandleCommand<SayHello> Members

        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="command">Command to run</param>
        public void Invoke(SayHello command)
        {
            Console.WriteLine("Hello");
        }

        #endregion
    }

    public class ErrorHandler : IUpstreamHandler
    {
        #region IUpstreamHandler Members

        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, object message)
        {
            Console.WriteLine(message);
        }

        #endregion
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var registrar = new ContainerRegistrar(Lifetime.Scoped);
            registrar.RegisterComponents(Lifetime.Default, Assembly.GetExecutingAssembly());
            var container = registrar.Build();


            var errorHandler = new ErrorHandler();

            new PipelineDispatcherBuilder(errorHandler)
                .AsyncDispatching(1)
                .RetryCommands(3)
                .UseRavenDbEmbedded()
                .UseGriffinContainer(container)
                .Build();

            CommandDispatcher.Dispatch(new SayHello());
            Console.ReadLine();
        }
    }
}