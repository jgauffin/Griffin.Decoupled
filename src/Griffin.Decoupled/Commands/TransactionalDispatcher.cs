using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Use transactions to prevent commands from being removed but not handled.
    /// </summary>
    public class TransactionalDispatcher : ICommandDispatcher
    {
        public TransactionalDispatcher(ICommandDispatcher inner, ITransactionalCommandStorage storage)
        {
            
        }
        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <typeparam name="T">Type of command</typeparam>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public void Dispatch(CommandState command)
        {
            
        }

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exiting.</remarks>
        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
