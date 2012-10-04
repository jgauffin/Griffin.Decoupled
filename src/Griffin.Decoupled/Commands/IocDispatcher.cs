using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Simple synchronous IoC dispatcher.
    /// </summary>
    public class IocDispatcher : ICommandDispatcher
    {
        private readonly IRootContainer _inversionOfControlContainer;

        public IocDispatcher(IRootContainer inversionOfControlContainer)
        {
            if (inversionOfControlContainer == null) throw new ArgumentNullException("inversionOfControlContainer");
            _inversionOfControlContainer = inversionOfControlContainer;
        }

        #region ICommandDispatcher Members

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch<T>(T command) where T : class, ICommand
        {
            if (command == null) throw new ArgumentNullException("command");
            _inversionOfControlContainer.Resolve<IHandleCommand<T>>().Invoke(command);
        }

        #endregion
    }
}