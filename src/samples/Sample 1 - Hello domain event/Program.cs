using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Decoupled;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.RavenDb;
using PipelineBuilder = Griffin.Decoupled.Commands.PipelineBuilder;

namespace Sample_1___Hello_domain_event
{
    public class SayHello : CommandBase
    { }

    [Component]
    public class SayHelloHandler : IHandleCommand<SayHello>
    {
        /// <summary>
        /// Invoke the command
        /// </summary>
        /// <param name="command">Command to run</param>
        public void Invoke(SayHello command)
        {
            Console.WriteLine("Hello");
        }
    }

    public class ErrorHandler : IUpstreamHandler
    {
        /// <summary>
        /// Send a message to the next handler
        /// </summary>
        /// <param name="context">My context</param>
        /// <param name="message">Message received</param>
        public void HandleUpstream(IUpstreamContext context, object message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var registrar = new ContainerRegistrar(Lifetime.Scoped);
            registrar.RegisterComponents(Lifetime.Default, Assembly.GetExecutingAssembly());
            var container = registrar.Build();


            var errorHandler = new ErrorHandler();

            new PipelineBuilder(errorHandler)
                .AsyncDispatching(1)
                .RetryCommands(3)
                .StoreCommandsInRavenDbEmbedded()
                .DispatchUsingGriffinContainer(container)
                .Build();

            CommandDispatcher.Dispatch(new SayHello());
            Console.ReadLine();
        }


    }
}
