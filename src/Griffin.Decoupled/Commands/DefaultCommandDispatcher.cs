using System;

namespace Griffin.Decoupled.Commands
{
    public abstract class DefaultCommandDispatcher : ICommandDispatcher
    {
        protected ICommandDispatcher _inner;
        protected ICommandStorage _storage;
        protected Action _dispatcherLoop;

        /// <summary>
        /// Dispatch the command to the handler
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <remarks>Implementations should throw exceptions unless they are asynchronous or will attempt to retry later.</remarks>
        public abstract void Dispatch(CommandState command);

        /// <summary>
        /// Close the dispatcher gracefully.
        /// </summary>
        /// <remarks>Should make sure that all non-persitent commands are executed or stored before exeting.</remarks>
        public abstract void Close();

        /// <summary>
        /// Dispatch as many commands as possible.
        /// </summary>
        protected void DispatchCommands()
        {
            
            var command = _storage.Dequeue();
            while (command != null)
            {
                _inner.Dispatch(command);
                command = _storage.Dequeue();
            }
        }

        /// <summary>
        /// Dispatch as many commands as possible.
        /// </summary>
        /// <remarks>This loop makes the command stay in the storage until the command has been completed successfully. That
        ///  makes our commands work even if the application crashes or fails in some other way.</remarks>
        protected void DispatchCommandsWithTransaction()
        {
            var transactional = (ITransactionalCommandStorage) _storage;
            while (true)
            {
                using (var transaction = transactional.BeginTransaction())
                {
                    var command = _storage.Dequeue();    
                    if (command == null)
                    {
                        transaction.Commit();
                        return;
                    }

                    _inner.Dispatch(command);
                    transaction.Commit();
                }
                
            }
        }
    }
}