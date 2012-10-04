using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Facade for the command handling
    /// </summary>
    /// <remarks>
    /// <para>You must configure the facade by assigning a dispatcher (typically <see cref="PipelineDispatcher"/>) using the <c>Assign()</c> method.</para>
    /// <para>Then you can just go ahead and use <c>CommandDispatcher.Dispatch(theCommand)</c> all over your code.</para>
    /// </remarks>
    public class CommandDispatcher
    {
        private static ICommandDispatcher _dispatcher;

        /// <summary>
        /// Assigns the specified domain event dispatcher.
        /// </summary>
        /// <param name="dispatcher">The command dispatcher.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Assign(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Dispatch an domain event
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">command to dispatch</param>
        /// <remarks>The command will either be dispatched synchronously or async depending on the used implementation.</remarks>
        public static void Dispatch<T>(T command) where T : class, ICommand
        {
            if (_dispatcher == null)
                throw new InvalidOperationException(
                    "A 'ICommandDispatcher' implementation have not been specified. Read the class documentation for the 'CommandDispatcher' class.");

            _dispatcher.Dispatch(command);
        }
    }
}