using System;
using Griffin.Container;
using Griffin.Decoupled.Commands;
using Griffin.Decoupled.Container;

// In root namespace to that it will show up in intellisense
namespace Griffin.Decoupled
{
    /// <summary>
    /// Used to add support for Griffin.Container
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Use Griffin.Container to dispatch events
        /// </summary>
        /// <param name="builder">this</param>
        /// <param name="griffinContainer">Your created container, look at the Griffin.Container HP for instructions on how to build it.</param>
        /// <returns>this</returns>
        public static PipelineDispatcherBuilder UseGriffinContainer(this PipelineDispatcherBuilder builder, IParentContainer griffinContainer)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (griffinContainer == null) throw new ArgumentNullException("griffinContainer");

            builder.UseContainer(new ContainerAdapter(griffinContainer));
            return builder;
        }

        /// <summary>
        /// Dispatch commands using our favorite container
        /// </summary>
        /// <param name="container">Container instance.</param>
        public static void DispatchCommands(this IParentContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            CommandDispatcher.Assign(new IocDispatcher(new ContainerAdapter(container)));
        }
    }
}