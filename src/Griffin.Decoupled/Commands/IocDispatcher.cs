using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Simple synchronous IoC dispatcher.
    /// </summary>
    /// <remarks>Important! Since this handler is synchronous you have to manage scoping by yourself.</remarks>
    public class IocDispatcher : ICommandDispatcher
    {
        private readonly IRootContainer _inversionOfControlContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocDispatcher" /> class.
        /// </summary>
        /// <param name="inversionOfControlContainer">The inversion of control container adapter.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
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

            using (var scope = _inversionOfControlContainer.CreateScope())
            {
                scope.Resolve<IHandleCommand<T>>().Invoke(command);    
            }
            
        }

        #endregion
    }
}