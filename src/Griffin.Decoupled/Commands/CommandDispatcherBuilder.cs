using System;

namespace Griffin.Decoupled.Commands
{
    /// <summary>
    /// Fluent dispatcher builder
    /// </summary>
    /// <remarks>
    /// <para>Used to simplify the process of building a command dispatcher.</para>
    /// </remarks>
    public class CommandDispatcherBuilder
    {
        private ICommandDispatcher _actualDispatcher;
        private Action<FailedCommandEventArgs> _failedCommands;
        private int _maxAttempts;
        private ICommandStorage _storage = new MemoryStorage();
        private Action<AsyncDispatcherExceptionEventArgs> _uncaughtExceptionsHandler;
        private int _workers;

        /// <summary>
        /// Store commands in a custom data storage
        /// </summary>
        /// <param name="storage">The storage</param>
        /// <returns>this</returns>
        /// <remarks>The memory is used per default</remarks>
        public CommandDispatcherBuilder StoreCommands(ICommandStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");

            _storage = storage;
            return this;
        }

        /// <summary>
        /// Make the dispatching asynchronous
        /// </summary>
        /// <param name="maxConcurrentCommands">Number of commands that can be executed simultaneously.</param>
        /// <param name="uncaughtExceptionsHandler">Invoked when an uncaught exceptions is detected on a dispatcher thread.</param>
        /// <returns>this</returns>
        public CommandDispatcherBuilder MakeAsync(int maxConcurrentCommands,
                                                  Action<AsyncDispatcherExceptionEventArgs> uncaughtExceptionsHandler)
        {
            if (uncaughtExceptionsHandler == null) throw new ArgumentNullException("uncaughtExceptionsHandler");
            if (maxConcurrentCommands < 0 || maxConcurrentCommands > 10)
                throw new ArgumentOutOfRangeException("maxConcurrentCommands", maxConcurrentCommands,
                                                      "1 to 100 is what we deem reasonable :O");

            _workers = maxConcurrentCommands;
            _uncaughtExceptionsHandler = uncaughtExceptionsHandler;
            return this;
        }

        /// <summary>
        /// Retry all failing commands before disposing them
        /// </summary>
        /// <param name="maxAttempts">Number of attempts for failing commands</param>
        /// <param name="failedCommands">Will be invoked when a command have failed all times</param>
        /// <returns>this</returns>
        public CommandDispatcherBuilder RetryCommands(int maxAttempts, Action<FailedCommandEventArgs> failedCommands)
        {
            if (failedCommands == null) throw new ArgumentNullException("failedCommands");
            if (maxAttempts < 0 || maxAttempts > 10)
                throw new ArgumentOutOfRangeException("maxAttempts", maxAttempts, "Attempts should be between 1 and 10.");

            _maxAttempts = maxAttempts;
            _failedCommands = failedCommands;
            return this;
        }

        /// <summary>
        /// Use an inversion of control container to locate the command handlers.
        /// </summary>
        /// <param name="container">IoC adapter.</param>
        /// <returns>this</returns>
        public CommandDispatcherBuilder UseContainer(IRootContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _actualDispatcher = new ContainerCommandDispatcher(container);
            return this;
        }


        /// <summary>
        /// Build the dispatcher.
        /// </summary>
        /// <returns>Created dispatcher</returns>
        public ICommandDispatcher Build()
        {
            if (_actualDispatcher == null)
                throw new InvalidOperationException(
                    "You must specify a dispatcher which can find handlers and invoke them. For instance by calling the UseContainer method.");

            var dispatcher = _actualDispatcher;
            if (_maxAttempts > 0)
            {
                var x = new RetryingDispatcher(dispatcher, _maxAttempts, _storage);
                x.CommandFailed += (sender, args) => _failedCommands(args);
                dispatcher = x;
            }

            if (_workers > 0)
            {
                var x = new AsyncCommandDispatcher(dispatcher, _storage, _workers);
                x.UncaughtException += (sender, args) => _uncaughtExceptionsHandler(args);
                dispatcher = x;
            }

            CommandDispatcher.Assign(dispatcher);
            return dispatcher;
        }

        /// <summary>
        /// Add a dispatcher which actually dispatches the commands.
        /// </summary>
        /// <param name="commandDispatcher">Custom dispatcher.</param>
        public CommandDispatcherBuilder Publisher(ICommandDispatcher commandDispatcher)
        {
            if (commandDispatcher == null) throw new ArgumentNullException("commandDispatcher");
            _actualDispatcher = commandDispatcher;
            return this;
        }
    }
}