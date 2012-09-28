using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Uses an inversion of control container to dispatch the commands
    /// </summary>
    public class ContainerCommandDispatcher : ICommandDispatcher
    {
        private readonly IRootContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerCommandDispatcher" /> class.
        /// </summary>
        /// <param name="container">The service locator (adapter for your favorite IoC container adapter).</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ContainerCommandDispatcher(IRootContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
        }

        #region ICommandDispatcher Members

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        public void Dispatch<T>(T command) where T : class, ICommand
        {
            if (command == null) throw new ArgumentNullException("command");

            using (var scope = _container.CreateScope())
            {
                foreach (var handler in scope.ResolveAll<IHandleCommand<T>>())
                {
                    handler.Invoke(command);
                }
            }
        }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public void Close()
        {
        }

        #endregion
    }
}