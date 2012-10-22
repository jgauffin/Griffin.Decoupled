using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Event arguments for the events in <see cref="IocDispatcher"/>
    /// </summary>
    public class IocScopeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IocScopeEventArgs" /> class.
        /// </summary>
        /// <param name="childScope">Created scope (i.e. child scope of the IoC container).</param>
        /// <param name="command">Command being executed.</param>
        public IocScopeEventArgs(IServiceLocator childScope, ICommand command)
        {
            ChildScope = childScope;
            Command = command;
        }

        /// <summary>
        /// Gets scope that was created for the command
        /// </summary>
        public IServiceLocator ChildScope { get; private set; }

        /// <summary>
        /// Gets command being executed.
        /// </summary>
        public ICommand Command { get; set; }
    }
}