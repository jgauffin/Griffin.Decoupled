using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Decoupled;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Commands.Pipeline.Messages;
using Griffin.Decoupled.RavenDb;

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

    class Program
    {
        static void Main(string[] args)
        {
            var registrar = new ContainerRegistrar(Lifetime.Scoped);
            registrar.RegisterComponents(Lifetime.Default, Assembly.GetExecutingAssembly());
            var container = registrar.Build();

            new CommandDispatcherBuilder()
                .MakeAsync(1, DispatcherFailed)
                .RetryCommands(3, HandleFailedCommands)
                .StoreCommandsInRavenDbEmbedded()
                .DispatchUsingGriffinContainer(container)
                .Build();

            CommandDispatcher.Dispatch(new SayHello());
            Console.ReadLine();
        }


        private static void HandleFailedCommands(CommandFailed e)
        {
            Console.WriteLine("The following command failed three times: " + e.Command);
        }

        private static void DispatcherFailed(AsyncDispatcherExceptionEventArgs e)
        {
            Console.WriteLine("The dispatcher failed to deliver a command with: " + e.Exception);
        }
    }
}
