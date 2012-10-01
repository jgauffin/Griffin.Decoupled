using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Used to simplify the process of building a command dispatcher.
    /// </summary>
    public class CommandDispatcherBuilder
    {
        private int _workers = 0;
        private int _maxAttempts;
        private ICommandStorage _storage = new MemoryStorage();
        private ICommandDispatcher _actualDispatcher;

        /// <summary>
        /// Store commands in a custom data storage
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public CommandDispatcherBuilder StoreCommands(ICommandStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            _storage = storage;
        }

        public CommandDispatcherBuilder MakeAsync(int maxConcurrentCommands)
        {
            _workers = maxConcurrentCommands;
            return this;
        }

        public CommandDispatcherBuilder RetryCommands(int maxAttempts)
        {
            _maxAttempts = maxAttempts;
            return this;
        }

        public CommandDispatcherBuilder UseContainer(IRootContainer container)
        {
            _actualDispatcher = new ContainerCommandDispatcher(container);
            return this;
        }

        public ICommandDispatcher Build()
        {
            if (_actualDispatcher == null)
                throw new InvalidOperationException("You must specify a dispatcher which can find handlers and invoke them. For instance by calling the UseContainer method.");

            var dispatcher = _actualDispatcher;
            if (_maxAttempts > 0)
                dispatcher = new RetryingDispatcher(dispatcher, _maxAttempts, _storage);

            if (_workers > 0)
                dispatcher = new AsyncDispatcher(dispatcher, _storage, _workers);

            return dispatcher;

        }
    }
}
