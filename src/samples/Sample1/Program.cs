using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Decoupled;
using Griffin.Decoupled.Commands;

namespace Sample1
{
    class Program
    {
        // This is the easiest way to gt started. 
        // Do note that the commands are invoked synchronous, but using a seperate IoC scope/child container for each invocation.
        //
        // Do not be tempted to return things from the commands even though you could.
        static void Main(string[] args)
        {
            var container = ConfigureGriffinContainer();

            // extension method from the Griffin.Decoupled.Container project.
            container.DispatchCommands();

            // Invoke that command.
            CommandDispatcher.Dispatch(new SayHello());    


            Console.ReadLine();
        }

        private static Container ConfigureGriffinContainer()
        {
            var registrar = new ContainerRegistrar(Lifetime.Scoped);
            registrar.RegisterComponents(Lifetime.Default, Assembly.GetExecutingAssembly());
            var container = registrar.Build();
            return container;
        }
    }
}
